﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:25
 * 
 */
using System.Collections.Generic;

namespace Hime.Redist.Parsers
{
    /// <summary>
    /// Represents a node in a Shared Packed Parse Forest
    /// </summary>
    public sealed class SPPFNode
    {
        private Symbol symbol;
        private SyntaxTreeNodeAction action;
        private int generation;
        private List<SPPFNodeFamily> families;

        /// <summary>
        /// Gets the symbol associated to this node
        /// </summary>
        public Symbol Symbol { get { return symbol; } }
        /// <summary>
        /// Gets or sets the action for this node
        /// </summary>
        public SyntaxTreeNodeAction Action
        {
            get { return action; }
            set { action = value; }
        }
        /// <summary>
        /// Gets the generation of this node
        /// </summary>
        public int Generation { get { return generation; } }

        /// <summary>
        /// Initializes a new instance of the SPPFNode class with the given symbol and generation
        /// </summary>
        /// <param name="symbol">The symbol represented by this node</param>
        /// <param name="gen">The generation of this node</param>
        public SPPFNode(Symbol symbol, int gen)
        {
            this.symbol = symbol;
            this.action = SyntaxTreeNodeAction.Nothing;
            this.generation = gen;
            this.families = new List<SPPFNodeFamily>();
        }
        /// <summary>
        /// Initializes a new instance of the SPPFNode class with the given symbol, generation and action
        /// </summary>
        /// <param name="symbol">The symbol represented by this node</param>
        /// <param name="gen">The generation of this node</param>
        /// <param name="action">The action for this node</param>
        public SPPFNode(Symbol symbol, int gen, SyntaxTreeNodeAction action)
        {
            this.symbol = symbol;
            this.action = action;
            this.generation = gen;
            this.families = new List<SPPFNodeFamily>();
        }

        /// <summary>
        /// Adds a new family for this node
        /// </summary>
        /// <param name="family">The new family for this node</param>
        public void AddFamily(SPPFNodeFamily family) { families.Add(family); }
        /// <summary>
        /// Adds a new family to be constructed from a list of nodes
        /// </summary>
        /// <param name="nodes">The list of nodes forming the new family</param>
        public void AddFamily(List<SPPFNode> nodes) { families.Add(new SPPFNodeFamily(this, nodes)); }

        /// <summary>
        /// Determines whether this node is equivalent to the given one
        /// </summary>
        /// <param name="node">The tested node</param>
        /// <returns>True if the given node is equivalent to this node, false otherwise</returns>
        public bool EquivalentTo(SPPFNode node)
        {
            if (this.symbol == null)
            {
                if (node.symbol != null)
                    return false;
                return (this.generation == node.generation);
            }
            else
            {
                if (!this.symbol.Equals(node.symbol))
                    return false;
                return (this.generation == node.generation);
            }
        }

        /// <summary>
        /// Determines whether this node has a family equivalent to the given one
        /// </summary>
        /// <param name="family">The tested family</param>
        /// <returns>True if this node has an equivalent family, false otherwise</returns>
        public bool HasEquivalentFamily(SPPFNodeFamily family)
        {
            foreach (SPPFNodeFamily potential in families)
                if (potential.EquivalentTo(family))
                    return true;
            return false;
        }

        /// <summary>
        /// Gets the first tree stemming from this SPPF
        /// </summary>
        /// <returns>The first tree stemming from this SPPF</returns>
        public SyntaxTreeNode GetFirstTree()
        {
            SyntaxTreeNode me = new SyntaxTreeNode(symbol, action);
            if (families.Count == 1)
            {
                foreach (SPPFNode child in families[0].Children)
                {
                    if (child.Symbol is SymbolAction)
                        ((SymbolAction)child.Symbol).Action.Invoke(me);
                    else
                        me.AppendChild(child.GetFirstTree());
                }
            }
            else if (families.Count >= 1)
            {
                // More than one solution => this is an error
                foreach (SPPFNodeFamily family in families)
                {
                    SyntaxTreeNode subroot = new SyntaxTreeNode(null, SyntaxTreeNodeAction.Nothing);
                    me.AppendChild(subroot);
                }
            }
            return me;
        }
    }
}
