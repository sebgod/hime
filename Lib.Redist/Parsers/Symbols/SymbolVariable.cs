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
    /// Represents a variable in an abstract syntax tree or a grammar
    /// </summary>
    public sealed class SymbolVariable : Symbol
    {
        /// <summary>
        /// Initializes a new instance of the SymbolVariable class with the given id and name
        /// </summary>
        /// <param name="sid">The unique ID of the symbol</param>
        /// <param name="name">The name of the symbol</param>
        public SymbolVariable(ushort sid, string name) : base(sid, name)
        {
        }
    }
}