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
    /// Represents a base LR parser
    /// </summary>
    public abstract class BaseLRParser : IParser
    {
        /// <summary>
        /// Maximal size of the stack
        /// </summary>
        protected const int maxStackSize = 128;
        /// <summary>
        /// Maximum number of errors
        /// </summary>
        protected const int maxErrorCount = 100;
        /// <summary>
        /// Determines whether the parser will try to recover from errors
        /// </summary>
        protected bool recover = true;

        /// <summary>
        /// Parser's variables
        /// </summary>
        protected SymbolDictionary<Symbols.Variable> parserVariables;
        /// <summary>
        /// Parser's virtuals
        /// </summary>
        protected SymbolDictionary<Symbols.Virtual> parserVirtuals;
        /// <summary>
        /// Parser's actions
        /// </summary>
        protected UserAction[] parserActions;
        /// <summary>
        /// List of the encountered syntaxic errors
        /// </summary>
        protected List<Error> allErrors;
        /// <summary>
        /// Lexer associated to this parser
        /// </summary>
        protected Lexer.TextLexer lexer;

        
        /// <summary>
        /// Gets the variable symbols used by this parser
        /// </summary>
        public SymbolDictionary<Symbols.Variable> Variables { get { return parserVariables; } }
        /// <summary>
        /// Gets the virtual symbols used by this parser
        /// </summary>
        public SymbolDictionary<Symbols.Virtual> Virtuals { get { return parserVirtuals; } }

        /// <summary>
        /// Gets or sets whether the paser should try to recover from errors
        /// </summary>
        public bool RecoverErrors
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
        protected BaseLRParser(Symbols.Variable[] variables, Symbols.Virtual[] virtuals, UserAction[] actions, Lexer.TextLexer lexer)
        {
            this.parserVariables = new SymbolDictionary<Symbols.Variable>(variables);
            this.parserVirtuals = new SymbolDictionary<Symbols.Virtual>(virtuals);
            this.parserActions = actions;
            this.recover = true;
            this.allErrors = new List<Error>();
            this.lexer = lexer;
            this.lexer.OnError += OnLexicalError;
        }

        /// <summary>
        /// Adds the given lexical error emanating from the lexer to the list of errors
        /// </summary>
        /// <param name="error">Lexical error</param>
        protected void OnLexicalError(Error error)
        {
            allErrors.Add(error);
        }

        /// <summary>
        /// Parses the input and returns the result
        /// </summary>
        /// <returns>A ParseResult object containing the data about the result</returns>
        public abstract ParseResult Parse();
    }
}
