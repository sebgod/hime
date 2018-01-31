/*******************************************************************************
 * Copyright (c) 2017 Association Cénotélie (cenotelie.fr)
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
 ******************************************************************************/

using System.Collections.Generic;
using Hime.Redist.Parsers;

namespace Hime.SDK.Grammars.LR
{
	/// <summary>
	/// Represents a set of reduction for a LR(1) state
	/// </summary>
	public class StateReductionsLR1 : StateReductions
	{
		/// <summary>
		/// Gets the set of the expected terminals in this set of reductions
		/// </summary>
		/// <remarks>This is always an empty set</remarks>
		public override TerminalSet ExpectedTerminals
		{
			get
			{
				TerminalSet result = new TerminalSet();
				foreach (StateActionReduce reduction in this)
					result.Add(reduction.Lookahead);
				return result;
			}
		}

		/// <summary>
		/// Build this set from the given LR state
		/// </summary>
		/// <param name="state">A LR state</param>
		public override void Build(State state)
		{
			// Recutions dictionnary for the given set
			Dictionary<Terminal, ItemLR1> reductions = new Dictionary<Terminal, ItemLR1>();
			// Construct reductions
			foreach (ItemLR1 item in state.Items)
			{
				if (item.Action == LRActionCode.Shift)
					continue;
				// There is already a shift action for the lookahead => conflict
				if (state.HasTransition(item.Lookahead))
					RaiseConflictShiftReduce(state, item, item.Lookahead);
				// There is already a reduction action for the lookahead => conflict
				else if (reductions.ContainsKey(item.Lookahead))
					RaiseConflictReduceReduce(state, item, reductions[item.Lookahead], item.Lookahead);
				else // No conflict
				{
					reductions.Add(item.Lookahead, item);
					Add(new StateActionReduce(item.Lookahead, item.BaseRule));
				}
			}
		}
	}
}
