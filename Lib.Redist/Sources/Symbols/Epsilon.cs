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

namespace Hime.Redist.Symbols
{
    /// <summary>
    /// Represents a special token for the absence of data in a stream
    /// </summary>
    public sealed class Epsilon : Token
    {
        private static Epsilon instance = new Epsilon();
        private Epsilon() : base(1, "ε") { }
        /// <summary>
        /// Gets the epsilon token
        /// </summary>
        public static Epsilon Instance { get { return instance; } }
        /// <summary>
        /// Gets the data represented by this symbol
        /// </summary>
        public override string Value { get { return string.Empty; } }
        /// <summary>
        /// Gets the position of this token in the input
        /// </summary>
        public override TextPosition Position { get { return new TextPosition(); } }
        /// <summary>
        /// Gets the length of the text in this token
        /// </summary>
        public override int Length { get { return 0; } }
    }
}