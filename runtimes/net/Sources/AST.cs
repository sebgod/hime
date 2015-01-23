﻿/**********************************************************************
* Copyright (c) 2014 Laurent Wouters and others
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

namespace Hime.Redist
{
	/// <summary>
	/// Represents an Abstract Syntax Tree produced by a parser
	/// </summary>
	interface AST
	{
		/// <summary>
		/// Gets the root node of this tree
		/// </summary>
		ASTNode Root { get; }

		/// <summary>
		/// Gets the number of children of the given node
		/// </summary>
		/// <param name="node">A node</param>
		/// <returns>The node's numer of children</returns>
		int GetChildrenCount(int node);

		/// <summary>
		/// Gets the i-th child of the given node
		/// </summary>
		/// <param name="parent">A node</param>
		/// <param name="i">The child's number</param>
		/// <returns>The i-th child</returns>
		ASTNode GetChild(int parent, int i);

		/// <summary>
		/// Gets an enumerator for the children of the given node
		/// </summary>
		/// <param name="parent">A node</param>
		/// <returns>An enumerator for the children</returns>
		IEnumerator<ASTNode> GetChildren(int parent);

		/// <summary>
		/// Gets the position in the input text of the given node
		/// </summary>
		/// <param name="node">A node</param>
		/// <returns>The position in the text</returns>
		TextPosition GetPosition(int node);

		/// <summary>
		/// Gets the span in the input text of the given node
		/// </summary>
		/// <param name="node">A node</param>
		/// <returns>The span in the text</returns>
		TextSpan GetSpan(int node);

		/// <summary>
		/// Gets the context in the input of the given node
		/// </summary>
		/// <param name="node">A node</param>
		/// <returns>The context</returns>
		TextContext GetContext(int node);

		/// <summary>
		/// Gets the grammar symbol associated to the given node
		/// </summary>
		/// <param name="node">A node</param>
		/// <returns>The associated symbol</returns>
		Symbol GetSymbol(int node);

		/// <summary>
		/// Gets the value of the given node
		/// </summary>
		/// <param name="node">A node</param>
		/// <returns>The associated value</returns>
		string GetValue(int node);
	}
}
