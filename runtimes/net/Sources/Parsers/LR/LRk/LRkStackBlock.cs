﻿/**********************************************************************
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

namespace Hime.Redist.Parsers
{
	/// <summary>
	/// Represents a block in the stack of LR(k) parser
	/// </summary>
	struct LRkStackBlock
	{
		/// <summary>
		/// The LR(k) automaton state associated with this block
		/// </summary>
		private int state;
		/// <summary>
		/// The contexts opened at this state
		/// </summary>
		private LRContexts contexts;

		/// <summary>
		/// Gets the LR(k) automaton state associated with this block
		/// </summary>
		public int State { get { return state; } }
		/// <summary>
		/// Gets the contexts opened at this state
		/// </summary>
		public LRContexts Contexts { get { return contexts; } }

		/// <summary>
		/// Setups this block
		/// </summary>
		/// <param name="state">The LR(k) automaton state associated with this block</param>
		/// <param name="contexts">The contexts opened at this state</param>
		public void Setup(int state, LRContexts contexts)
		{
			this.state = state;
			this.contexts = contexts;
		}
	}
}
