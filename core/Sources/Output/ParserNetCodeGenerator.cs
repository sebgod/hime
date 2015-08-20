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

namespace Hime.SDK.Output
{
	/// <summary>
	/// Represents a generator for parser code for the .Net platform
	/// </summary>
	public class ParserNetCodeGenerator : Generator
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
		/// The grammar to generate a parser for
		/// </summary>
		private readonly Grammars.Grammar grammar;
		/// <summary>
		/// The type of the parser to generate
		/// </summary>
		private readonly string parserType;
		/// <summary>
		/// The type of the automaton
		/// </summary>
		private readonly string automatonType;

		/// <summary>
		/// Initializes this code generator
		/// </summary>
		/// <param name="unit">The unit to generate code for</param>
		/// <param name="binResource">Path to the automaton's binary resource</param>
		public ParserNetCodeGenerator(Unit unit, string binResource)
		{
			nmespace = unit.Namespace;
			modifier = unit.Modifier;
			name = unit.Name;
			this.binResource = binResource;
			grammar = unit.Grammar;
			if (unit.Method == ParsingMethod.RNGLR1 || unit.Method == ParsingMethod.RNGLALR1)
			{
				parserType = "RNGLRParser";
				automatonType = "RNGLRAutomaton";
			}
			else
			{
				parserType = "LRkParser";
				automatonType = "LRkAutomaton";
			}
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
			StreamWriter writer = new StreamWriter(file, false, new System.Text.UTF8Encoding(false));

			WriteHeader(writer);

			writer.WriteLine("using System.Collections.Generic;");
			writer.WriteLine("using Hime.Redist;");
			writer.WriteLine("using Hime.Redist.Parsers;");
			writer.WriteLine();
			writer.WriteLine("namespace " + nmespace);
			writer.WriteLine("{");
			writer.WriteLine("\t/// <summary>");
			writer.WriteLine("\t/// Represents a parser");
			writer.WriteLine("\t/// </summary>");
			writer.WriteLine("\t" + modifier.ToString().ToLower() + " class " + name + "Parser : " + parserType);
			writer.WriteLine("\t{");

			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// The automaton for this parser");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\tprivate static readonly " + automatonType + " commonAutomaton = " + automatonType + ".Find(typeof(" + name + "Parser), \"" + binResource + "\");");

			GenerateCodeSymbols(writer);
			GenerateCodeVariables(writer);
			GenerateCodeVirtuals(writer);
			GenerateCodeActions(writer);
			GeneratorCodeConstructors(writer);

			writer.WriteLine("\t}");
			writer.WriteLine("}");
			writer.Close();
		}

		/// <summary>
		/// Generates the code for the symbols
		/// </summary>
		/// <param name="stream">The output stream</param>
		private void GenerateCodeSymbols(StreamWriter stream)
		{
			Dictionary<Grammars.Variable, string> nameVariables = new Dictionary<Grammars.Variable, string>();
			Dictionary<Grammars.Virtual, string> nameVirtuals = new Dictionary<Grammars.Virtual, string>();
			foreach (Grammars.Variable var in grammar.Variables)
			{
				if (var.Name.StartsWith(Grammars.Grammar.PREFIX_GENERATED_VARIABLE))
					continue;
				nameVariables.Add(var, Helper.SanitizeNameCS(var.Name));
			}
			foreach (Grammars.Virtual var in grammar.Virtuals)
			{
				string name = Helper.SanitizeNameCS(var.Name);
				while (nameVariables.ContainsValue(name) || nameVirtuals.ContainsValue(name))
					name += Helper.VIRTUAL_SUFFIX;
				nameVirtuals.Add(var, name);
			}

			stream.WriteLine("\t\t/// <summary>");
			stream.WriteLine("\t\t/// Contains the constant IDs for the variables and virtuals in this parser");
			stream.WriteLine("\t\t/// </summary>");
			stream.WriteLine("\t\tpublic class ID");
			stream.WriteLine("\t\t{");
			foreach (KeyValuePair<Grammars.Variable, string> pair in nameVariables)
			{
				stream.WriteLine("\t\t\t/// <summary>");
				stream.WriteLine("\t\t\t/// The unique identifier for variable " + pair.Key.Name);
				stream.WriteLine("\t\t\t/// </summary>");
				stream.WriteLine("\t\t\tpublic const int {0} = 0x{1};", pair.Value, pair.Key.ID.ToString("X4"));
			}
			foreach (KeyValuePair<Grammars.Virtual, string> pair in nameVirtuals)
			{
				stream.WriteLine("\t\t\t/// <summary>");
				stream.WriteLine("\t\t\t/// The unique identifier for virtual " + pair.Key.Name);
				stream.WriteLine("\t\t\t/// </summary>");
				stream.WriteLine("\t\t\tpublic const int {0} = 0x{1};", pair.Value, pair.Key.ID.ToString("X4"));
			}
			stream.WriteLine("\t\t}");
		}

		/// <summary>
		/// Generates the code for the variables
		/// </summary>
		/// <param name="stream">The output stream</param>
		private void GenerateCodeVariables(StreamWriter stream)
		{
			stream.WriteLine("\t\t/// <summary>");
			stream.WriteLine("\t\t/// The collection of variables matched by this parser");
			stream.WriteLine("\t\t/// </summary>");
			stream.WriteLine("\t\t/// <remarks>");
			stream.WriteLine("\t\t/// The variables are in an order consistent with the automaton,");
			stream.WriteLine("\t\t/// so that variable indices in the automaton can be used to retrieve the variables in this table");
			stream.WriteLine("\t\t/// </remarks>");
			stream.WriteLine("\t\tprivate static readonly Symbol[] variables = {");
			bool first = true;
			foreach (Grammars.Variable var in grammar.Variables)
			{
				if (!first)
					stream.WriteLine(", ");
				stream.Write("\t\t\t");
				stream.Write("new Symbol(0x" + var.ID.ToString("X4") + ", \"" + var.Name + "\")");
				first = false;
			}
			stream.WriteLine(" };");
		}

		/// <summary>
		/// Generates the code for the virtual symbols
		/// </summary>
		/// <param name="stream">The output stream</param>
		private void GenerateCodeVirtuals(StreamWriter stream)
		{
			stream.WriteLine("\t\t/// <summary>");
			stream.WriteLine("\t\t/// The collection of virtuals matched by this parser");
			stream.WriteLine("\t\t/// </summary>");
			stream.WriteLine("\t\t/// <remarks>");
			stream.WriteLine("\t\t/// The virtuals are in an order consistent with the automaton,");
			stream.WriteLine("\t\t/// so that virtual indices in the automaton can be used to retrieve the virtuals in this table");
			stream.WriteLine("\t\t/// </remarks>");
			stream.WriteLine("\t\tprivate static readonly Symbol[] virtuals = {");
			bool first = true;
			foreach (Grammars.Virtual v in grammar.Virtuals)
			{
				if (!first)
					stream.WriteLine(", ");
				stream.Write("\t\t\t");
				stream.Write("new Symbol(0x" + v.ID.ToString("X4") + ", \"" + v.Name + "\")");
				first = false;
			}
			stream.WriteLine(" };");
		}

		/// <summary>
		/// Generates the code for the semantic actions
		/// </summary>
		/// <param name="stream">The output stream</param>
		private void GenerateCodeActions(StreamWriter stream)
		{
			if (grammar.Actions.Count == 0)
				return;
			stream.WriteLine("\t\t/// <summary>");
			stream.WriteLine("\t\t/// Represents a set of semantic actions in this parser");
			stream.WriteLine("\t\t/// </summary>");
			stream.WriteLine("\t\tpublic class Actions");
			stream.WriteLine("\t\t{");
			foreach (Grammars.Action action in grammar.Actions)
			{
				stream.WriteLine("\t\t\t/// <summary>");
				stream.WriteLine("\t\t\t/// The " + action.Name + " semantic action");
				stream.WriteLine("\t\t\t/// </summary>");
				stream.WriteLine("\t\t\tpublic virtual void " + action.Name + "(Symbol head, SemanticBody body) { }");
			}
			stream.WriteLine();
			stream.WriteLine("\t\t}");

			stream.WriteLine("\t\t/// <summary>");
			stream.WriteLine("\t\t/// Represents a set of empty semantic actions (do nothing)");
			stream.WriteLine("\t\t/// </summary>");
			stream.WriteLine("\t\tprivate static readonly Actions noActions = new Actions();");

			stream.WriteLine("\t\t/// <summary>");
			stream.WriteLine("\t\t/// Gets the set of semantic actions in the form a table consistent with the automaton");
			stream.WriteLine("\t\t/// </summary>");
			stream.WriteLine("\t\t/// <param name=\"input\">A set of semantic actions</param>");
			stream.WriteLine("\t\t/// <returns>A table of semantic actions</returns>");
			stream.WriteLine("\t\tprivate static SemanticAction[] GetUserActions(Actions input)");
			stream.WriteLine("\t\t{");
			stream.WriteLine("\t\t\tSemanticAction[] result = new SemanticAction[" + grammar.Actions.Count + "];");
			int i = 0;
			foreach (Grammars.Action action in grammar.Actions)
			{
				stream.WriteLine("\t\t\tresult[" + i + "] = new SemanticAction(input." + action.Name + ");");
				i++;
			}
			stream.WriteLine("\t\t\treturn result;");
			stream.WriteLine("\t\t}");

			stream.WriteLine("\t\t/// <summary>");
			stream.WriteLine("\t\t/// Gets the set of semantic actions in the form a table consistent with the automaton");
			stream.WriteLine("\t\t/// </summary>");
			stream.WriteLine("\t\t/// <param name=\"input\">A set of semantic actions</param>");
			stream.WriteLine("\t\t/// <returns>A table of semantic actions</returns>");
			stream.WriteLine("\t\tprivate static SemanticAction[] GetUserActions(Dictionary<string, SemanticAction> input)");
			stream.WriteLine("\t\t{");
			stream.WriteLine("\t\t\tSemanticAction[] result = new SemanticAction[" + grammar.Actions.Count + "];");
			i = 0;
			foreach (Grammars.Action action in grammar.Actions)
			{
				stream.WriteLine("\t\t\tresult[" + i + "] = input[\"" + action.Name + "\"];");
				i++;
			}
			stream.WriteLine("\t\t\treturn result;");
			stream.WriteLine("\t\t}");
		}

		/// <summary>
		/// Generates the code for the constructors
		/// </summary>
		/// <param name="stream">The output stream</param>
		private void GeneratorCodeConstructors(StreamWriter stream)
		{
			if (grammar.Actions.Count == 0)
			{
				stream.WriteLine("\t\t/// <summary>");
				stream.WriteLine("\t\t/// Initializes a new instance of the parser");
				stream.WriteLine("\t\t/// </summary>");
				stream.WriteLine("\t\t/// <param name=\"lexer\">The input lexer</param>");
				stream.WriteLine("\t\tpublic " + name + "Parser(" + name + "Lexer lexer) : base (commonAutomaton, variables, virtuals, null, lexer) { }");
			}
			else
			{
				stream.WriteLine("\t\t/// <summary>");
				stream.WriteLine("\t\t/// Initializes a new instance of the parser");
				stream.WriteLine("\t\t/// </summary>");
				stream.WriteLine("\t\t/// <param name=\"lexer\">The input lexer</param>");
				stream.WriteLine("\t\tpublic " + name + "Parser(" + name + "Lexer lexer) : base (commonAutomaton, variables, virtuals, GetUserActions(noActions), lexer) { }");

				stream.WriteLine("\t\t/// <summary>");
				stream.WriteLine("\t\t/// Initializes a new instance of the parser");
				stream.WriteLine("\t\t/// </summary>");
				stream.WriteLine("\t\t/// <param name=\"lexer\">The input lexer</param>");
				stream.WriteLine("\t\t/// <param name=\"actions\">The set of semantic actions</param>");
				stream.WriteLine("\t\tpublic " + name + "Parser(" + name + "Lexer lexer, Actions actions) : base (commonAutomaton, variables, virtuals, GetUserActions(actions), lexer) { }");

				stream.WriteLine("\t\t/// <summary>");
				stream.WriteLine("\t\t/// Initializes a new instance of the parser");
				stream.WriteLine("\t\t/// </summary>");
				stream.WriteLine("\t\t/// <param name=\"lexer\">The input lexer</param>");
				stream.WriteLine("\t\t/// <param name=\"actions\">The set of semantic actions</param>");
				stream.WriteLine("\t\tpublic " + name + "Parser(" + name + "Lexer lexer, Dictionary<string, SemanticAction> actions) : base (commonAutomaton, variables, virtuals, GetUserActions(actions), lexer) { }");
			}
		}
	}
}
