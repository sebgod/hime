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
using Hime.Redist.Utils;

namespace Hime.SDK.Grammars.LR
{
	/// <summary>
	/// Represents a set of reduction in a LR state
	/// </summary>
	public abstract class StateReductions : IEnumerable<StateActionReduce>
	{
		/// <summary>
		/// The reductions in this set
		/// </summary>
		private readonly List<StateActionReduce> content;
		/// <summary>
		/// The conflicts raised by this set
		/// </summary>
		private readonly List<Conflict> conflicts;

		/// <summary>
		/// Gets the number of reductions in this set
		/// </summary>
		public int Count { get { return content.Count; } }

		/// <summary>
		/// Gets the reductions a the specifie index
		/// </summary>
		/// <param name='index'>An index</param>
		public StateActionReduce this[int index] { get { return content[index]; } }

		/// <summary>
		/// Gets the conflicts raised by this set
		/// </summary>
		public ROList<Conflict> Conflicts { get { return new ROList<Conflict>(conflicts); } }

		/// <summary>
		/// Initializes this set of reductions as empty
		/// </summary>
		protected StateReductions()
		{
			content = new List<StateActionReduce>();
			conflicts = new List<Conflict>();
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator</returns>
		public IEnumerator<StateActionReduce> GetEnumerator()
		{
			return content.GetEnumerator();
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return content.GetEnumerator();
		}

		/// <summary>
		/// Adds a reduction to this set
		/// </summary>
		/// <param name="action">The reduction to add</param>
		public void Add(StateActionReduce action)
		{
			content.Add(action);
		}

		/// <summary>
		/// Gets the set of the expected terminals in this set of reductions
		/// </summary>
		public abstract TerminalSet ExpectedTerminals { get; }

		/// <summary>
		/// Build this set from the given LR state
		/// </summary>
		/// <param name="state">A LR state</param>
		public abstract void Build(State state);

		/// <summary>
		/// Raises a shift/reduce conflict
		/// </summary>
		/// <param name="state">The parent state</param>
		/// <param name="item">The conflictuous reduction item</param>
		/// <param name="lookahead">The lookahead creating the conflict</param>
		protected void RaiseConflictShiftReduce(State state, Item item, Terminal lookahead)
		{
			// Look for previous conflict
			foreach (Conflict previous in conflicts)
			{
				if (previous.ErrorType == ErrorType.ConflictShiftReduce && previous.ConflictSymbol == lookahead)
				{
					// Previous conflict
					previous.AddItem(item);
					return;
				}
			}
			// No previous conflict was found
			Conflict conflict = new Conflict(state, ErrorType.ConflictShiftReduce, lookahead);
			foreach (Item potential in state.Items)
				if (potential.Action == LRActionCode.Shift && potential.GetNextSymbol().ID == lookahead.ID)
					conflict.AddItem(potential);
			conflict.AddItem(item);
			conflicts.Add(conflict);
		}

		/// <summary>
		/// Raises a reduce/reduce conflict
		/// </summary>
		/// <param name="state">The parent state</param>
		/// <param name="currentItem">The conflictuous reduction item (current)</param>
		/// <param name="previousItem">The conflictuous reduction item (previous)</param>
		/// <param name="lookahead">The lookahead creating the conflict</param>
		protected void RaiseConflictReduceReduce(State state, Item currentItem, Item previousItem, Terminal lookahead)
		{
			// Look for previous conflict
			foreach (Conflict previous in conflicts)
			{
				if (previous.ErrorType == ErrorType.ConflictReduceReduce && previous.ConflictSymbol == lookahead)
				{
					// Previous conflict
					previous.AddItem(currentItem);
					return;
				}
			}
			// No previous conflict was found
			Conflict conflict = new Conflict(state, ErrorType.ConflictReduceReduce, lookahead);
			conflict.AddItem(previousItem);
			conflict.AddItem(currentItem);
			conflicts.Add(conflict);
		}
	}
}