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
    /// Represents an action symbol in a shared packed parse forest
    /// </summary>
    public sealed class SymbolAction : SymbolVirtual
    {
        /// <summary>
        /// Represents the method to call for executing the action
        /// </summary>
        /// <param name="subroot">The syntax tree node on which the action is executed</param>
        public delegate void Callback(SyntaxTreeNode subroot);

        public Callback Action { get; private set; }

        public SymbolAction(string name, Callback callback): base(name)
        {
            this.Action = callback;
        }
    }
}