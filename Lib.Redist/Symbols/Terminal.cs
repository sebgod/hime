﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:25
 * 
 */
using System.Collections.Generic;

namespace Hime.Redist.Symbols
{
    /// <summary>
    /// Represents a terminal in a grammar
    /// </summary>
    public sealed class Terminal : Symbol
    {
        /// <summary>
        /// Initializes a new instance of the Terminal class with the given ID and name
        /// </summary>
        /// <param name="sid">Symbol's unique identifier</param>
        /// <param name="name">Symbol's name</param>
        public Terminal(ushort sid, string name) : base(sid, name) { }
    }
}