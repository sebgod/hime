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
    /// Represents an error in a lexer
    /// </summary>
    public interface LexerError
    {
        /// <summary>
        /// Gets the error's message
        /// </summary>
        string Message { get; }
    }
}