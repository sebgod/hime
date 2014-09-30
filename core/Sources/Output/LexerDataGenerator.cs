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
using System.IO;

namespace Hime.CentralDogma.Output
{
	/// <summary>
	/// Represents a generator of data and code for a lexer
	/// </summary>
	public class LexerDataGenerator : Generator
	{
		/// <summary>
		/// The terminals matched by the lexer
		/// </summary>
		private ROList<Grammars.Terminal> terminals;
		/// <summary>
		/// The contexts for the lexer
		/// </summary>
		private ROList<Grammars.Variable> contexts;
		/// <summary>
		/// The lexer's DFA
		/// </summary>
		private Automata.DFA dfa;

		/// <summary>
		/// Initializes this generator
		/// </summary>
		/// <param name="dfa">The dfa to serialize</param>
		public LexerDataGenerator(Automata.DFA dfa, ROList<Grammars.Terminal> expected, ROList<Grammars.Variable> contexts)
		{
			this.dfa = dfa;
			this.terminals = expected;
			this.contexts = contexts;
		}

		/// <summary>
		/// Generates the lexer's binary data
		/// </summary>
		/// <param name="file">The file to output to</param>
		public void Generate(string file)
		{
			BinaryWriter writer = new BinaryWriter(new FileStream(file, FileMode.Create));

			writer.Write((uint)dfa.StatesCount);
			uint offset = 0;
			foreach (Automata.DFAState state in dfa.States)
			{
				writer.Write(offset);
				offset += 3 + 256;
				foreach (CharSpan key in state.Transitions)
					if (key.End >= 256)
						offset += 3;
			}
			foreach (Automata.DFAState state in dfa.States)
				GenerateDataFor(writer, state);

			writer.Close();
		}

		/// <summary>
		/// Generates the given state binary data
		/// </summary>
		/// <param name="writer">The output writer</param>
		/// <param name="state">The state to export</param>
		private void GenerateDataFor(BinaryWriter writer, Automata.DFAState state)
		{
			// build the transition data
			ushort[] cache = new ushort[256];
			for (int i = 0; i != 256; i++)
				cache[i] = 0xFFFF;
			ushort cached = 0; // the number of cached transitions
			ushort slow = 0; // the number of non-cached transitions
			foreach (CharSpan span in state.Transitions)
			{
				if (span.Begin <= 255)
				{
					cached++;
					int end = span.End;
					if (end >= 256)
					{
						end = 255;
						slow++;
					}
					for (int i = span.Begin; i <= end; i++)
						cache[i] = (ushort)state.GetChildBy(span).ID;
				}
				else
					slow++;
			}

			// build the matched terminals data
			List<ushort> contexts = new List<ushort>();
			List<ushort> matched = new List<ushort>();
			foreach (Automata.FinalItem item in state.Items)
			{
				Grammars.Terminal terminal = item as Grammars.Terminal;
				ushort context = GetContextID(terminal.Context);
				if (!contexts.Contains(context))
				{
					// this is the first time this context is found in the current DFA state
					// this is the terminal with the most priority for this context
					contexts.Add(context);
					matched.Add((ushort)terminals.IndexOf(terminal));
				}
			}

			// write the number of matched terminals
			writer.Write((ushort)matched.Count);
			// write the total numer of transitions
			writer.Write((ushort)(slow + cached));
			// write the number of non-cached transitions
			writer.Write(slow);
			// write the matched terminals
			for (int i = 0; i != matched.Count; i++)
			{
				writer.Write(contexts[i]);
				writer.Write(matched[i]);
			}
			// write the cached transitions
			for (int i = 0; i != 256; i++)
				writer.Write(cache[i]);
			// write the non-cached transitions
			List<CharSpan> keys = new List<CharSpan>(state.Transitions);
			keys.Sort(new System.Comparison<CharSpan>(CharSpan.CompareReverse));
			foreach (CharSpan span in keys)
			{
				if (span.End <= 255)
					break; // the rest of the transitions are in the cache
				ushort begin = span.Begin;
				if (begin <= 255)
					begin = 256;
				writer.Write(begin);
				writer.Write(System.Convert.ToUInt16(span.End));
				writer.Write((ushort)state.GetChildBy(span).ID);
			}
		}

		/// <summary>
		/// Gets the identifier fo the specified context's name
		/// </summary>
		/// <param name="name">The name of a context</param>
		/// <returns>The corresponding identifier</returns>
		private ushort GetContextID(string name)
		{
			if (name == null)
				return 0;
			for (ushort i = 1; i != contexts.Count; i++)
			{
				if (contexts[i].Name == name)
					return i;
			}
			return 0xFFFF;
		}
	}
}
