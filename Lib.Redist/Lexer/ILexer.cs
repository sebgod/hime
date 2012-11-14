﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hime.Redist.Parsers
{
    /// <summary>
    /// Handler for lexical errors
    /// </summary>
    /// <param name="error">The new error</param>
    public delegate void AddLexicalError(ParserError error);

    /// <summary>
    /// Represents a lexer
    /// </summary>
    public interface ILexer : ITokenStream
    {
        /// <summary>
        /// Gets the terminals matched by this lexer
        /// </summary>
        Utils.SymbolDictionary<SymbolTerminal> Terminals { get; }

        /// <summary>
        /// Gets the current line number
        /// </summary>
        int CurrentLine { get; }

        /// <summary>
        /// Gets the current column number
        /// </summary>
        int CurrentColumn { get; }

        /// <summary>
        /// Events for lexical errors
        /// </summary>
        event AddLexicalError OnError;
    }
}
