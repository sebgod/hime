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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Hime.Redist
{
    /// <summary>
    /// Represents an unexpected token error in a parser
    /// </summary>
    public sealed class UnexpectedTokenError : Error
    {
        /// <summary>
        /// Gets the unexpected token
        /// </summary>
        public Token UnexpectedToken { get; private set; }

        /// <summary>
        /// Gets a list of the expected terminals
        /// </summary>
        public IList<Symbol> ExpectedTerminals { get; private set; }

		/// <summary>
        /// Initializes a new instance of the UnexpectedTokenError class with a token and an array of expected names
		/// </summary>
		/// <param name='token'>The unexpected token</param>
		/// <param name='expected'>The expected terminals</param>
        /// <param name="text">The text containing the token</param>
        internal UnexpectedTokenError(Token token, IList<Symbol> expected, TokenizedText text)
            : base(ErrorType.UnexpectedToken, text.GetPositionOf(token))
        {
            this.UnexpectedToken = token;
            this.ExpectedTerminals = new ReadOnlyCollection<Symbol>(expected);
            StringBuilder Builder = new StringBuilder("Unexpected token \"");
            Builder.Append(text.GetValue(token));
            Builder.Append("\"; expected: { ");
            for (int i = 0; i != expected.Count; i++)
            {
                if (i != 0) Builder.Append(", ");
                Builder.Append(expected[i].Name);
            }
            Builder.Append(" }");
            this.Message += Builder.ToString();
        }
    }
}