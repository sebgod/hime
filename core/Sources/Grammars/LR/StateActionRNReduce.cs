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

namespace Hime.SDK.Grammars.LR
{
	/// <summary>
	/// Represents a reduction action in a RNGLR state
	/// </summary>
	public sealed class StateActionRNReduce : StateActionReduce
	{
		/// <summary>
		/// Gets the reduction length
		/// </summary>
		public int ReduceLength { get; private set; }

		/// <summary>
		/// Initializes this action
		/// </summary>
		/// <param name="lookahead">The lookahead to reduce on</param>
		/// <param name="rule">The rule to reduce</param>
		/// <param name="length">The length of the reduction</param>
		public StateActionRNReduce(Terminal lookahead, Rule rule, int length) : base(lookahead, rule)
		{
			ReduceLength = length;
		}
	}
}