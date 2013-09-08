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

namespace Hime.Redist
{
    /// <summary>
    /// Represents a node in an Abstract Syntax Tree
    /// </summary>
    public struct ASTNode
    {
        private ParseTree tree;
        private int index;

        /// <summary>
        /// Gets the symbol in this node
        /// </summary>
        public Symbols.Symbol Symbol { get { return tree.GetSymbolAt(index); } }

        /// <summary>
        /// Gets the children of this node
        /// </summary>
        public ASTFamily Children { get { return new ASTFamily(tree, index); } }

        internal ASTNode(ParseTree tree, int index)
        {
            this.tree = tree;
            this.index = index;
        }

        /// <summary>
        /// Gets a string representation of this node
        /// </summary>
        /// <returns>The name of the current node's symbol; or "null" if the node does not have a symbol</returns>
        public override string ToString()
        {
            Symbols.Symbol symbol = tree.GetSymbolAt(index);
            if (symbol != null)
                return symbol.ToString();
            else
                return "null";
        }
    }
}
