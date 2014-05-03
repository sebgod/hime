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
using Hime.Redist.Parsers;

namespace Hime.CentralDogma.Grammars.LR
{
	/// <summary>
	/// Represents a LR(1) item
	/// </summary>
	public class ItemLR1 : Item
	{
		/// <summary>
		/// The lookahead for this item
		/// </summary>
		private Terminal lookahead;
		/// <summary>
		/// The lookaheads for this item (just the one)
		/// </summary>
		private TerminalSet lookaheads;

		/// <summary>
		///  Gets the lookahead for this item
		/// </summary>
		public Terminal Lookahead { get { return lookahead; } }
		/// <summary>
		///  Gets the lookaheads for this item
		/// </summary>
		public override TerminalSet Lookaheads { get { return lookaheads; } }

		/// <summary>
		/// Initializes this item
		/// </summary>
		/// <param name="rule">The underlying rule</param>
		/// <param name="position">The dot position in the rule</param>
		/// <param name="lookahead">The lookahead for this item</param>
		public ItemLR1(Rule rule, int position, Terminal lookahead) : base(rule, position)
		{
			this.lookahead = lookahead;
			this.lookaheads = new TerminalSet();
			this.lookaheads.Add(lookahead);
		}

		/// <summary>
		///  Gets the child of this item
		/// </summary>
		/// <returns>The child of this item</returns>
		public override Item GetChild()
		{
			if (Action == LRActionCode.Reduce)
				return null;
			return new ItemLR1(rule, position + 1, lookahead);
		}

		/// <summary>
		/// Closes this item to a set of items
		/// </summary>
		/// <param name="closure">The list to close</param>
		/// <param name="map">The current helper map</param>
		public override void CloseTo(List<Item> closure, Dictionary<Rule, Dictionary<int, List<Item>>> map)
		{
			// the item was of the form [Var -> alpha .] (reduction)
			// nothing to do
			if (Action == LRActionCode.Reduce)
				return;
			// Get the next symbol in the item
			Symbol next = GetNextSymbol();
			// Here the item is of the form [Var -> alpha . Next beta]
			// If the next symbol is not a variable : do nothing
			// If the next symbol is a variable :
			Variable nextVar = next as Variable;
			if (nextVar == null)
				return;
			// Firsts is a copy of the Firsts set for beta (next choice)
			// Firsts will contains symbols that may follow Next
			// Firsts will therefore be the lookahead for child items
			TerminalSet firsts = new TerminalSet(GetNextChoice().Firsts);
			// If beta is nullifiable (contains ε) :
			if (firsts.Contains(Epsilon.Instance))
			{
				// Remove ε
				firsts.Remove(Epsilon.Instance);
				// Add the item's lookahead as possible symbol for firsts
				firsts.Add(lookahead);
			}
			// For each rule that has Next as a head variable :
			foreach (Rule rule in nextVar.Rules)
			{
				if (!map.ContainsKey(rule))
					map.Add(rule, new Dictionary<int, List<Item>>());
				Dictionary<int, List<Item>> sub = map[rule];
				if (!sub.ContainsKey(0))
					sub.Add(0, new List<Item>());
				List<Item> previouses = sub[0];
				// For each symbol in Firsts : create the child with this symbol as lookahead
				foreach (Terminal first in firsts)
				{
					// Child item creation and unique insertion
					int sid = first.ID;
					bool found = false;
					foreach (Item previous in previouses)
					{
						if (previous.Lookaheads[0].ID == sid)
						{
							found = true;
							break;
						}
					}
					if (!found)
					{
						ItemLR1 New = new ItemLR1(rule, 0, first);
						closure.Add(New);
						previouses.Add(New);
					}
				}
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="Hime.CentralDogma.Grammars.LR.Item"/> is equal to the current <see cref="Hime.CentralDogma.Grammars.LR.Item"/>.
		/// </summary>
		/// <param name='obj'>
		/// The <see cref="Hime.CentralDogma.Grammars.LR.Item"/> to compare with the current <see cref="Hime.CentralDogma.Grammars.LR.Item"/>.
		/// </param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="Hime.CentralDogma.Grammars.LR.Item"/> is equal to the current
		/// <see cref="Hime.CentralDogma.Grammars.LR.Item"/>; otherwise, <c>false</c>.
		/// </returns>
		public override bool ItemEquals(Item item)
		{
			ItemLR1 tested = item as ItemLR1;
			if (this.lookahead.ID != tested.lookahead.ID)
				return false;
			return BaseEquals(tested);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Hime.CentralDogma.Grammars.LR.Item"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents the current <see cref="Hime.CentralDogma.Grammars.LR.Item"/>.
		/// </returns>
		public override string ToString()
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder("[");
			builder.Append(rule.Head.ToString());
			builder.Append(" ->");
			int i = 0;
			foreach (RuleBodyElement Part in rule.Body.Choices[0])
			{
				if (i == position)
					builder.Append(" " + dot);
				builder.Append(" ");
				builder.Append(Part.ToString());
				i++;
			}
			if (i == position)
				builder.Append(" " + dot);
			builder.Append(", ");
			builder.Append(lookahead.ToString());
			builder.Append("]");
			return builder.ToString();
		}
	}
}
