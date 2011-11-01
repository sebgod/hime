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
    /// Handler for lexical errors
    /// </summary>
    /// <param name="error"></param>
    public delegate void OnErrorHandler(ParserError error);

    /// <summary>
    /// Represents a lexer
    /// </summary>
    public interface ILexer
    {
		// TODO: instead of having a line and column, should have a Position!!
		// Binary position is number of characters since start
		// Text position is line and column
        /// <summary>
        /// Gets the current line number in the input
        /// </summary>
        int CurrentLine { get; }
        /// <summary>
        /// Gets the current column number in the input
        /// </summary>
        int CurrentColumn { get; }

		// TODO: think about it, but a Clone method is not a good sign. What is it needed for?
        /// <summary>
        /// Gets a clone of this lexer
        /// </summary>
        /// <returns>A clone of this lexer</returns>
        ILexer Clone();
        /// <summary>
        /// Gets the next token in the input
        /// </summary>
        /// <returns>The next token in the input</returns>
        SymbolToken GetNextToken();
        /// <summary>
        /// Get the next token in the input that has is of one of the provided IDs
        /// </summary>
        /// <param name="ids">The possible IDs of the next expected token</param>
        /// <returns>The next token in the input</returns>
        SymbolToken GetNextToken(ushort[] ids);

        /// <summary>
        /// Sets the handler for lexical errors
        /// </summary>
        OnErrorHandler OnError { set; }
    }
}