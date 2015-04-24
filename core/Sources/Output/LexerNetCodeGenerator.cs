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
using System.IO;
using Hime.Redist.Utils;
using Hime.SDK.Grammars;

namespace Hime.SDK.Output
{
	/// <summary>
	/// Represents a generator for lexer code for the .Net platform
	/// </summary>
	public sealed class LexerNetCodeGenerator : Generator
	{
		/// <summary>
		/// The nmespace of the generated code
		/// </summary>
		private readonly string nmespace;
		/// <summary>
		/// The visibility modifier for the generated code
		/// </summary>
		private readonly Modifier modifier;
		/// <summary>
		/// The name of the generated lexer
		/// </summary>
		private readonly string name;
		/// <summary>
		/// Path to the automaton's binary resource
		/// </summary>
		private readonly string binResource;
		/// <summary>
		/// The terminals for the lexer
		/// </summary>
		private readonly ROList<Terminal> terminals;
		/// <summary>
		/// The contexts for the lexer
		/// </summary>
		private readonly ROList<Variable> contexts;
		/// <summary>
		/// The separator terminal
		/// </summary>
		private readonly Terminal separator;

		/// <summary>
		/// Initializes this code generator
		/// </summary>
		/// <param name="unit">The unit to generate code for</param>
		/// <param name="binResource">Path to the automaton's binary resource</param>
		public LexerNetCodeGenerator(Unit unit, string binResource)
		{
			nmespace = unit.Namespace;
			modifier = unit.Modifier;
			name = unit.Name;
			this.binResource = binResource;
			terminals = unit.Expected;
			contexts = unit.Contexts;
			separator = unit.Separator;
		}

		/// <summary>
		/// Writes a generated .Net file header
		/// </summary>
		/// <param name="writer">The writer to write to</param>
		private void WriteHeader(StreamWriter writer)
		{
			writer.WriteLine("/*");
			writer.WriteLine(" * WARNING: this file has been generated by");
			writer.WriteLine(" * Hime Parser Generator " + CompilationTask.Version);
			writer.WriteLine(" */");
		}

		/// <summary>
		/// Generates code for the specified file
		/// </summary>
		/// <param name="file">The target file to generate code in</param>
		public void Generate(string file)
		{
			string baseLexer = contexts.Count > 1 ? "ContextSensitiveLexer" : "ContextFreeLexer";
			StreamWriter writer = new StreamWriter(file, false, new System.Text.UTF8Encoding(false));

			WriteHeader(writer);

			writer.WriteLine("using System.Collections.Generic;");
			writer.WriteLine("using System.IO;");
			writer.WriteLine("using Hime.Redist;");
			writer.WriteLine("using Hime.Redist.Lexer;");
			writer.WriteLine();
			writer.WriteLine("namespace " + nmespace);
			writer.WriteLine("{");

			writer.WriteLine("\t/// <summary>");
			writer.WriteLine("\t/// Represents a lexer");
			writer.WriteLine("\t/// </summary>");
			writer.WriteLine("\t" + modifier.ToString().ToLower() + " class " + name + "Lexer : " + baseLexer);
			writer.WriteLine("\t{");

			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// The automaton for this lexer");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\tprivate static readonly Automaton commonAutomaton = Automaton.Find(typeof(" + name + "Lexer), \"" + binResource + "\");");

			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// Contains the constant IDs for the terminals for this lexer");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\tpublic class ID");
			writer.WriteLine("\t\t{");
			for (int i = 2; i != terminals.Count; i++)
			{
				Terminal terminal = terminals[i];
				if (terminal.Name.StartsWith(Grammar.PREFIX_GENERATED_TERMINAL))
					continue;
				writer.WriteLine("\t\t\t/// <summary>");
				writer.WriteLine("\t\t\t/// The unique identifier for terminal " + terminal.Name);
				writer.WriteLine("\t\t\t/// </summary>");
				writer.WriteLine("\t\t\tpublic const int {0} = 0x{1};", Helper.SanitizeNameCS(terminal), terminal.ID.ToString("X4"));
			}
			writer.WriteLine("\t\t}");

			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// Contains the constant IDs for the contexts for this lexer");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\tpublic class Context");
			writer.WriteLine("\t\t{");
			writer.WriteLine("\t\t\t/// <summary>");
			writer.WriteLine("\t\t\t/// The unique identifier for the default context");
			writer.WriteLine("\t\t\t/// </summary>");
			writer.WriteLine("\t\t\tpublic const int DEFAULT = 0;");
			for (int i = 1; i != contexts.Count; i++)
			{
				Variable context = contexts[i];
				writer.WriteLine("\t\t\t/// <summary>");
				writer.WriteLine("\t\t\t/// The unique identifier for context " + context.Name);
				writer.WriteLine("\t\t\t/// </summary>");
				writer.WriteLine("\t\t\tpublic const int {0} = 0x{1};", Helper.SanitizeNameCS(context), i.ToString("X4"));
			}
			writer.WriteLine("\t\t}");

			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// The collection of terminals matched by this lexer");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\t/// <remarks>");
			writer.WriteLine("\t\t/// The terminals are in an order consistent with the automaton,");
			writer.WriteLine("\t\t/// so that terminal indices in the automaton can be used to retrieve the terminals in this table");
			writer.WriteLine("\t\t/// </remarks>");
			writer.WriteLine("\t\tprivate static readonly Symbol[] terminals = {");
			bool first = true;
			foreach (Terminal terminal in terminals)
			{
				if (!first)
					writer.WriteLine(",");
				writer.Write("\t\t\t");
				writer.Write("new Symbol(0x" + terminal.ID.ToString("X4") + ", \"" + terminal.ToString().Replace("\"", "\\\"") + "\")");
				first = false;
			}
			writer.WriteLine(" };");

			string sep = "FFFF";
			if (separator != null)
				sep = separator.ID.ToString("X4");
			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// Initializes a new instance of the lexer");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\t/// <param name=\"input\">The lexer's input</param>");
			writer.WriteLine("\t\tpublic " + name + "Lexer(string input) : base(commonAutomaton, terminals, 0x" + sep + ", input) {}");

			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// Initializes a new instance of the lexer");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\t/// <param name=\"input\">The lexer's input</param>");
			writer.WriteLine("\t\tpublic " + name + "Lexer(TextReader input) : base(commonAutomaton, terminals, 0x" + sep + ", input) {}");

			writer.WriteLine("\t}");
			writer.WriteLine("}");

			writer.Close();
		}
	}
}