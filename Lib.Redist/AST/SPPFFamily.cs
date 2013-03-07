﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:25
 * 
 */
using System.Collections.Generic;

namespace Hime.Redist.AST
{
    /// <summary>
    /// Represents a family of children for a SPPF node
    /// </summary>
    public sealed class SPPFFamily
    {
        private SPPFNode parent;
        private List<SPPFNode> children;

        /// <summary>
        /// Gets the SPPF node owning this family
        /// </summary>
        public SPPFNode Parent { get { return parent; } }
        /// <summary>
        /// Gets the list of children in this family
        /// </summary>
        public IList<SPPFNode> Children { get { return children; } }

        /// <summary>
        /// Initializes a new instance of the SPPFFamily class with the given parent node
        /// </summary>
        /// <param name="parent">The node owning this family</param>
        public SPPFFamily(SPPFNode parent)
        {
            this.parent = parent;
            this.children = new List<SPPFNode>();
        }
        /// <summary>
        /// Initializes a new instance of the SPPFFamily class with the given parent node
        /// </summary>
        /// <param name="parent">The node owning this family</param>
        /// <param name="nodes">The list of children for this family</param>
        public SPPFFamily(SPPFNode parent, IEnumerable<SPPFNode> nodes)
        {
            this.parent = parent;
            this.children = new List<SPPFNode>(nodes);
        }

        /// <summary>
        /// Adds a new child to this family
        /// </summary>
        /// <param name="child">The child to add to this family</param>
        public void AddChild(SPPFNode child) { children.Add(child); }

        /// <summary>
        /// Determines whether this family is equivalent to the tested one
        /// </summary>
        /// <param name="family">The tested family</param>
        /// <returns>True if this family is equivalent to the tested one, false otherwise</returns>
        public bool EquivalentTo(SPPFFamily family)
        {
            if (children.Count != family.children.Count)
                return false;
            for (int i = 0; i != children.Count; i++)
                if (!children[i].EquivalentTo(family.children[i]))
                    return false;
            return true;
        }
    }
}