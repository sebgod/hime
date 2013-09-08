/**********************************************************************
* Copyright (c) 2013 Laurent Wouters and others
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU Lesser General Public License as
* published by the Free Software Foundation, either version 3
* of the License, or (at your option) any later version.
* 
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU Lesser General Public License for more details.
* 
* You should have received a copy of the GNU Lesser General
* Public License along with this program.
* If not, see <http://www.gnu.org/licenses/>.
* 
* Contributors:
*     Laurent Wouters - lwouters@xowl.org
**********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Hime.Redist.Parsers
{
    /// <summary>
    /// Delegate for a semantic action on the given body and with the given parent
    /// </summary>
    /// <param name="head">The semantic object for the head</param>
    /// <param name="body">The current body at the time of the action</param>
    /// <param name="length">The length of the passed body</param>
    public delegate void SemanticAction(Symbols.Variable head, Symbols.Symbol[] body, int length);

    /// <summary>
    /// Represents a base LR parser
    /// </summary>
    public abstract class BaseLRParser : IParser
    {
        /// <summary>
        /// Maximal size of the stack
        /// </summary>
        protected int maxStackSize = 100;
        /// <summary>
        /// Maximum number of errors
        /// </summary>
        protected int maxErrorCount = 100;
        /// <summary>
        /// Maximum lenght of a rule
        /// </summary>
        protected int maxBodyLength = 20;
        /// <summary>
        /// Determines whether the parser will try to recover from errors
        /// </summary>
        protected bool recover = true;
        /// <summary>
        /// Determines whether the parser will execute the semantic actions
        /// </summary>
        protected bool executeActions = true;

        /// <summary>
        /// Parser's variables
        /// </summary>
        protected Utils.SymbolDictionary<Symbols.Variable> parserVariables;
        /// <summary>
        /// Parser's virtuals
        /// </summary>
        protected Utils.SymbolDictionary<Symbols.Virtual> parserVirtuals;
        /// <summary>
        /// Parser's actions
        /// </summary>
        protected SemanticAction[] parserActions;
        /// <summary>
        /// List of the encountered syntaxic errors
        /// </summary>
        protected List<ParserError> allErrors;
        /// <summary>
        /// Read-only list of the errors
        /// </summary>
        protected System.Collections.ObjectModel.ReadOnlyCollection<ParserError> readonlyErrors;
        /// <summary>
        /// Lexer associated to this parser
        /// </summary>
        protected Lexer.TextLexer lexer;

        
        /// <summary>
        /// Gets the variable symbols used by this parser
        /// </summary>
        public Utils.SymbolDictionary<Symbols.Variable> Variables { get { return parserVariables; } }
        /// <summary>
        /// Gets the virtual symbols used by this parser
        /// </summary>
        public Utils.SymbolDictionary<Symbols.Virtual> Virtuals { get { return parserVirtuals; } }
        /// <summary>
        /// Gets a read-only collection of syntaxic errors encountered by the parser
        /// </summary>
        public ICollection<ParserError> Errors { get { return readonlyErrors; } }
        /// <summary>
        /// Gets or sets whether the parser should try to recover from errors or fails immediatly
        /// </summary>
        public bool TryRecover
        {
            get { return recover; }
            set { recover = value; }
        }

        /// <summary>
        /// Initializes a new instance of the LRkParser class with the given lexer
        /// </summary>
        /// <param name="variables">The parser's variables</param>
        /// <param name="virtuals">The parser's virtuals</param>
        /// <param name="actions">The parser's actions</param>
        /// <param name="lexer">The input lexer</param>
        protected BaseLRParser(Symbols.Variable[] variables, Symbols.Virtual[] virtuals, SemanticAction[] actions, Lexer.TextLexer lexer)
        {
            this.parserVariables = new Utils.SymbolDictionary<Symbols.Variable>(variables);
            this.parserVirtuals = new Utils.SymbolDictionary<Symbols.Virtual>(virtuals);
            this.parserActions = actions;
            this.recover = true;
            this.allErrors = new List<ParserError>();
            this.readonlyErrors = new System.Collections.ObjectModel.ReadOnlyCollection<ParserError>(allErrors);
            this.lexer = lexer;
            this.lexer.OnError += OnLexicalError;
        }

        /// <summary>
        /// Adds the given lexical error emanating from the lexer to the list of errors
        /// </summary>
        /// <param name="error">Lexical error</param>
        protected void OnLexicalError(ParserError error)
        {
            allErrors.Add(error);
        }

        /// <summary>
        /// Parses the input and returns the produced AST
        /// </summary>
        /// <returns>AST produced by the parser representing the input, or null if unrecoverable errors were encountered</returns>
        public abstract AST.ASTNode Parse();
    }
}
