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
	/// Represents a state in a Deterministic Finite Automaton
	/// </summary>
	public class DFAState
    {
		/// <summary>
		/// The transitions from this state
		/// </summary>
        private Dictionary<CharSpan, DFAState> transitions;
        
        /// <summary>
        /// The top marker for this state
        /// </summary>
        private FinalItem item;
        
        /// <summary>
        /// The list of the current markers for this state
        /// </summary>
        private List<FinalItem> items;
        
        /// <summary>
        /// This state's id
        /// </summary>
        private int id;

        /// <summary>
        /// Gets the top marker for this state
        /// </summary>
        public FinalItem TopItem { get { return item; } }
        
        /// <summary>
        /// Gets the list of the markers for this state
        /// </summary>
        public List<FinalItem> Items { get { return items; } }
        
        /// <summary>
        /// Gets the transitions from this state
        /// </summary>
        public Dictionary<CharSpan, DFAState> Transitions { get { return transitions; } }
        
        /// <summary>
        /// Gets the number of markers for this state
        /// </summary>
        public int FinalsCount { get { return items.Count; } }
        
        /// <summary>
        /// Gets the number of transitions from this state
        /// </summary>
        public int TransitionsCount { get { return transitions.Count; } }
        
        /// <summary>
        /// Gets the ID of this state
        /// </summary>
        public int ID { get { return id; } }

        /// <summary>
        /// Initialize this state
        /// </summary>
        /// <param name="id">Identifier for this state</param>
        public DFAState(int id)
        {
            this.items = new List<FinalItem>();
            this.transitions = new Dictionary<CharSpan, DFAState>();
            this.id = id;
        }

        /// <summary>
        /// Adds a new marker making this state a final state
        /// </summary>
        /// <param name="item">The new marker</param>
        public void AddFinal(FinalItem item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                if (this.item == null)
                    this.item = item;
                else
                {
                    if (item.Priority > this.item.Priority)
                        this.item = item;
                }
            }
        }
        
        /// <summary>
        /// Adds new markers making this state a final state
        /// </summary>
        /// <param name="items">The new markers</param>
        public void AddFinals(ICollection<FinalItem> items)
        {
            foreach (FinalItem item in items)
                AddFinal(item);
        }
        
        /// <summary>
        /// Clears all markers for this states making it non-final
        /// </summary>
        public void ClearFinals()
        {
        	items.Clear();
        	item = null;
        }

        /// <summary>
        /// Adds a transition from this state
        /// </summary>
        /// <param name="value">The value on the transition</param>
        /// <param name="next">The next state by the transition</param>
        public void AddTransition(CharSpan value, DFAState next) { transitions.Add(value, next); }
        
        /// <summary>
        /// Removes all the transitions from this state
        /// </summary>
        public void ClearTransitions() { transitions.Clear(); }

        /// <summary>
        /// Repacks all the transitions from this state to remove overlaps between the transitions' values
        /// </summary>
        public void RepackTransitions()
        {
            Dictionary<DFAState, List<CharSpan>> inverse = new Dictionary<DFAState, List<CharSpan>>();
            foreach (KeyValuePair<CharSpan, DFAState> transition in transitions)
            {
                if (!inverse.ContainsKey(transition.Value))
                    inverse.Add(transition.Value, new List<CharSpan>());
                inverse[transition.Value].Add(transition.Key);
            }
            transitions.Clear();
            foreach (DFAState child in inverse.Keys)
            {
                List<CharSpan> keys = inverse[child];
                keys.Sort(new System.Comparison<CharSpan>(CharSpan.Compare));
                for (int i = 0; i != keys.Count; i++)
                {
                    CharSpan k1 = keys[i];
                    for (int j = i + 1; j != keys.Count; j++)
                    {
                        CharSpan k2 = keys[j];
                        if (k2.Begin == k1.End + 1)
                        {
                            k1 = new CharSpan(k1.Begin, k2.End);
                            keys[i] = k1;
                            keys.RemoveAt(j);
                            j--;
                        }
                    }
                }
                foreach (CharSpan key in keys)
                    transitions.Add(key, child);
            }
        }
    }
}