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
using Hime.CentralDogma.Grammars;
using System.IO;

namespace Hime.CentralDogma.Output
{
	/// <summary>
	/// Represents a generator for lexer code for the .Net platform
	/// </summary>
	public class LexerNetCodeGenerator : Generator
	{
		/// <summary>
		/// The nmespace of the generated code
		/// </summary>
		private string nmespace;
		/// <summary>
		/// The visibility modifier for the generated code
		/// </summary>
		private Modifier modifier;
		/// <summary>
		/// The name of the generated lexer
		/// </summary>
		private string name;
		/// <summary>
		/// Path to the automaton's binary resource
		/// </summary>
		private string binResource;
		/// <summary>
		/// The terminals for the lexer
		/// </summary>
		private ROList<Terminal> terminals;
		/// <summary>
		/// The separator terminal
		/// </summary>
		private Terminal separator;

		/// <summary>
		/// Initializes this code generator
		/// </summary>
		/// <param name="nmespace">The nmespace of the generated code</param>
		/// <param name="modifier">The visibility modifier for the generated code</param>
		/// <param name="name">The name of the generated lexer</param>
		/// <param name="binResource">Path to the automaton's binary resource</param>
		/// <param name="terminals">The terminals for the lexer</param>
		/// <param name="separator">The separator terminal</param>
		public LexerNetCodeGenerator(string nmespace, Modifier modifier, string name, string binResource, ROList<Terminal> terminals, Terminal separator)
		{
			this.nmespace = nmespace;
			this.modifier = modifier;
			this.name = name;
			this.binResource = binResource;
			this.terminals = terminals;
			this.separator = separator;
		}

		/// <summary>
		/// Writes a generated .Net file header
		/// </summary>
		/// <param name="writer">The writer to write to</param>
		protected void WriteHeader(StreamWriter writer)
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
			writer.WriteLine("\t" + modifier.ToString().ToLower() + " class " + name + "Lexer : PrefetchedLexer");
			writer.WriteLine("\t{");

			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// The automaton for this lexer");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\tprivate static readonly Automaton automaton = Automaton.Find(typeof(" + name + "Lexer), \"" + binResource + "\");");

			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// Contains the constant IDs for the terminals for this lexer");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\tpublic class ID");
			writer.WriteLine("\t\t{");
			for (int i = 2; i != terminals.Count; i++)
			{
				Grammars.Terminal terminal = terminals[i];
				if (terminal.Name.StartsWith(Grammars.Grammar.prefixGeneratedTerminal))
					continue;
				writer.WriteLine("\t\t\t/// <summary>");
				writer.WriteLine("\t\t\t/// The unique identifier for terminal " + terminal.Name);
				writer.WriteLine("\t\t\t/// </summary>");
				writer.WriteLine("\t\t\tpublic const int {0} = 0x{1};", Helper.SanitizeName(terminal), terminal.ID.ToString("X4"));
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
			foreach (Grammars.Terminal terminal in terminals)
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
			writer.WriteLine("\t\tpublic " + name + "Lexer(string input) : base(automaton, terminals, 0x" + sep + ", input) {}");

			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// Initializes a new instance of the lexer");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\t/// <param name=\"input\">The lexer's input</param>");
			writer.WriteLine("\t\tpublic " + name + "Lexer(TextReader input) : base(automaton, terminals, 0x" + sep + ", input) {}");

			writer.WriteLine("\t}");
			writer.WriteLine("}");

			writer.Close();
		}
	}
}