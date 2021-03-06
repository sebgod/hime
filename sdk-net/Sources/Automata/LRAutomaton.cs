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
using Hime.Redist.Utils;

namespace Hime.SDK.Automata
{
	/// <summary>
	/// Represents an LR automaton
	/// </summary>
	public class LRAutomaton
	{
		/// <summary>
		/// The states
		/// </summary>
		private readonly List<LRState> states;

		/// <summary>
		/// Gets the states in this automaton
		/// </summary>
		public ROList<LRState> States { get { return new ROList<LRState>(states); } }

		/// <summary>
		/// Initializes an empty automaton
		/// </summary>
		public LRAutomaton()
		{
			states = new List<LRState>();
		}

		/// <summary>
		/// Adds the specified state to this automaton
		/// </summary>
		/// <param name="state">A state</param>
		public void AddState(LRState state)
		{
			states.Add(state);
		}
	}
}