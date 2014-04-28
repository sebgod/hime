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

namespace Hime.CentralDogma.Automata
{
	/// <summary>
	/// Represents a state in a Non-deterministic Finite Automaton
	/// </summary>
	public class NFAState
	{
		/// <summary>
		/// The list of transitions
		/// </summary>
		private List<NFATransition> transitions;
		/// <summary>
		/// The marker whether this state is a final state
		/// </summary>
		private FinalItem item;
		/// <summary>
		/// The watermark for this state
		/// </summary>
		private int mark;

		/// <summary>
		/// Gets the list of transitions from this state
		/// </summary>
		public List<NFATransition> Transitions { get { return transitions; } }

		/// <summary>
		/// Gets or sets the item marking this state as a final state
		/// If the value is null, the state is not final
		/// </summary>
		public FinalItem Item
		{
			get { return item; }
			set { item = value; }
		}

		/// <summary>
		/// Gets or sets the watermark of this state
		/// </summary>
		public int Mark
		{
			get { return mark; }
			set { mark = value; }
		}

		/// <summary>
		/// Initializes this state as a non-final, non-watermarked state
		/// </summary>
		public NFAState()
		{
			transitions = new List<NFATransition>();
			item = null;
			mark = 0;
		}

		/// <summary>
		/// Adds a transition from this state to the given state on the given value
		/// </summary>
		/// <param name="value">The new transition's value</param>
		/// <param name="next">The next state by the new transition</param>
		public void AddTransition(CharSpan value, NFAState next)
		{
			transitions.Add(new NFATransition(value, next));
		}

		/// <summary>
		/// Removes all transitions starting from this state
		/// </summary>
		public void ClearTransitions()
		{
			transitions.Clear();
		}
	}
}