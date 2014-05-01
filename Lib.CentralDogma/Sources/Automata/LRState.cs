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
using Hime.Redist;

namespace Hime.CentralDogma.Automata
{
	/// <summary>
	/// Represents a state in an LR automaton
	/// </summary>
	public class LRState
	{
		/// <summary>
		/// The state's identifier
		/// </summary>
		private int id;
		/// <summary>
		/// The transitions from this state
		/// </summary>
		private List<LRTransition> transitions;
		/// <summary>
		/// The reductions in this state
		/// </summary>
		private List<LRReduction> reductions;
		/// <summary>
		/// Whether this state is an accepting state
		/// </summary>
		private bool accept;

		/// <summary>
		/// Gets this state's identifier
		/// </summary>
		public int ID { get { return id; } }
		/// <summary>
		/// Gets the transitions from this state
		/// </summary>
		public List<LRTransition> Transitions { get { return transitions; } }
		/// <summary>
		/// Gets the reductions in this state
		/// </summary>
		public List<LRReduction> Reductions { get { return reductions; } }
		/// <summary>
		/// Gets or sets whether this state is an accepting state
		/// </summary>
		public bool IsAccept
		{
			get { return accept; }
			set { accept = value; }
		}

		/// <summary>
		/// Initializes this LR state
		/// </summary>
		/// <param name="id">The state's identifier</param>
		public LRState(int id)
		{
			this.id = id;
			this.transitions = new List<LRTransition>();
			this.reductions = new List<LRReduction>();
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Hime.CentralDogma.Automata.LRState"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents the current <see cref="Hime.CentralDogma.Automata.LRState"/>.
		/// </returns>
		public override string ToString()
		{
			return string.Format("({0})", id);
		}
	}
}