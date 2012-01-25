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
    /// Represents a terminal in a grammar
    /// </summary>
    public sealed class SymbolTerminal : Symbol
    {
        /// <summary>
        /// Initializes a new instance of the SymbolTerminal class with the given type ID and name
        /// </summary>
        /// <param name="sid">Symbol's unique type identifier</param>
        /// <param name="name">Symbol's type name</param>
        public SymbolTerminal(ushort sid, string name) : base(sid, name) { }
    }
}