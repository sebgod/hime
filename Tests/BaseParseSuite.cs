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

using System;
using System.IO;
using System.Reflection;
using Hime.CentralDogma;
using Hime.Redist.AST;
using Hime.Redist.Symbols;
using NUnit.Framework;

namespace Hime.Tests
{
	/// <summary>
	/// Represents a base test suite for integration parsing test
	/// </summary>
	public abstract class BaseParseSuite : BaseTestSuite
	{
		/// <summary>
		/// The global random source
		/// </summary>
		private static Random rand = new Random();

		/// <summary>
		/// Gets a unique prefix
		/// </summary>
		/// <returns>A unique prefix</returns>
		private static string GetUniquePrefix()
		{
			int i1 = rand.Next();
			int i2 = rand.Next();
			int i3 = rand.Next();
			int i4 = rand.Next();
			return i1.ToString("X8") + "_" + i2.ToString("X8") + "_" + i3.ToString("X8") + "_" + i4.ToString("X8");
		}

		/// <summary>
		/// Constructor for lexers of parse trees
		/// </summary>
		private ConstructorInfo parseTreeLexer;
		/// <summary>
		/// Constructor for parsers of parse trees
		/// </summary>
        private ConstructorInfo parseTreeParser;
        
        /// <summary>
        /// Initializes this test suite
        /// </summary>
        protected BaseParseSuite()
        {
			ExportResource("ParseTree.gram", "ParseTree.gram");
			Hime.HimeCC.Program.Main(new string[] { "ParseTree.gram -o:nosources -a:public -n Hime.Tests.Generated" });
        	Assembly assembly = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, "ParseTree.dll"));
            Type tl = assembly.GetType("Hime.Tests.Generated.ParseTreeLexer");
            Type tp = assembly.GetType("Hime.Tests.Generated.ParseTreeParser");
            parseTreeLexer = tl.GetConstructor(new Type[] { typeof(string) });
            parseTreeParser = tp.GetConstructor(new Type[] { tl });
        }

        /// <summary>
        /// Parses the string representation of the given parse tree
        /// </summary>
        /// <param name="data">A string representation of a parse tree</param>
        /// <returns>The parse tree's AST</returns>
        private ASTNode ParseTree(string data)
        {
        	Hime.Redist.Lexer.ILexer lexer = parseTreeLexer.Invoke(new object[] { data }) as Hime.Redist.Lexer.ILexer;
            Hime.Redist.Parsers.IParser parser = parseTreeParser.Invoke(new object[] { lexer }) as Hime.Redist.Parsers.IParser;
            return parser.Parse();
        }
        
        /// <summary>
        /// Compare two parse trees
        /// </summary>
        /// <param name="expected">The expected sub tree</param>
        /// <param name="node">The sub tree to compare</param>
        /// <returns>True if the two trees match</returns>
        private bool Compare(ASTNode expected, ASTNode node)
        {
            if (node.Symbol.Name != (expected.Symbol as Token).Value)
                return false;
            if (expected.Children[0].Children.Count != 0)
            {
                string vRef = (expected.Children[0].Children[0].Symbol as Token).Value;
                vRef = vRef.Substring(1, vRef.Length - 2);
                string vReal = (node.Symbol as Token).Value;
                if (vReal != vRef)
                    return false;
            }
            if (node.Children.Count != expected.Children[1].Children.Count)
                return false;
            for (int i = 0; i != node.Children.Count; i++)
                if (!Compare(expected.Children[1].Children[i], node.Children[i]))
                    return false;
            return true;
        }

		/// <summary>
        /// Builds a parser
        /// </summary>
        /// <param name="grammars">Grammar content</param>
        /// <param name="top">The top grammar to compile</param>
        /// <param name="method">The parsing method to use</param>
		/// <param name="input">The input text to parse</param>
        /// <param name="prefix">Prefix for the generated parser</param>
		/// <returns>The parser</returns>
        protected Hime.Redist.Parsers.IParser BuildParser(string grammars, string top, ParsingMethod method, string input, string prefix)
		{
			string genNamespace = "Hime.Tests.Generated_" + prefix;

        	CompilationTask task = new CompilationTask();
            task.AddInputRaw(grammars);
			task.GrammarName = top;
            task.CodeAccess = AccessModifier.Public;
            task.Method = method;
            task.Mode = CompilationMode.Assembly;
            task.Namespace = genNamespace;
			task.OutputPrefix = prefix;
            Hime.CentralDogma.Reporting.Report report = task.Execute();
            Assert.AreEqual(0, report.ErrorCount, "Failed to compile the grammar");
            Assert.IsTrue(CheckFileExists(prefix + ".dll"), "Failed to produce the assembly");

			Assembly assembly = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, prefix + ".dll"));
            Type typeLexer = assembly.GetType(genNamespace + "." + top + "Lexer");
            Type typeParser = assembly.GetType(genNamespace + "." + top + "Parser");
			ConstructorInfo ciLexer = typeLexer.GetConstructor(new Type[] { typeof(string) });
            ConstructorInfo ciParser = typeParser.GetConstructor(new Type[] { typeLexer });

			Hime.Redist.Lexer.ILexer lexer = ciLexer.Invoke(new object[] { input }) as Hime.Redist.Lexer.ILexer;
            Hime.Redist.Parsers.IParser parser = ciParser.Invoke(new object[] { lexer }) as Hime.Redist.Parsers.IParser;
			return parser;
		}
        
        /// <summary>
        /// Tests whether the given grammar parses the input as the expected AST
        /// </summary>
        /// <param name="grammars">Grammar content</param>
        /// <param name="top">The top grammar to compile</param>
        /// <param name="method">The parsing method to use</param>
        /// <param name="input">The input text to parse</param>
        /// <param name="expected">The expected AST</param>
		protected void ParsingMatches(string grammars, string top, ParsingMethod method, string input, string expected)
        {
			Hime.Redist.Parsers.IParser parser = BuildParser(grammars, top, method, input, GetUniquePrefix());
			ASTNode inputAST = parser.Parse();
			foreach (Hime.Redist.Parsers.ParserError error in parser.Errors)
				Console.WriteLine(error.ToString());
			Assert.AreEqual(0, parser.Errors.Count, "Failed to parse the input");
			Assert.IsNotNull(inputAST, "Failed to parse the input");
			ASTNode expectedAST = ParseTree(expected);
			Assert.IsNotNull(expectedAST, "Failed to parse the expected tree");

			bool result = Compare(expectedAST, inputAST);
			Assert.IsTrue(result, "AST from input does not match the expected AST");
        }

		/// <summary>
        /// Tests whether the given grammar does not parse the input as the expected AST
        /// </summary>
        /// <param name="grammars">Grammar content</param>
        /// <param name="top">The top grammar to compile</param>
        /// <param name="method">The parsing method to use</param>
        /// <param name="input">The input text to parse</param>
        /// <param name="unexpected">The expected AST</param>
		protected void ParsingNotMatches(string grammars, string top, ParsingMethod method, string input, string unexpected)
        {
			Hime.Redist.Parsers.IParser parser = BuildParser(grammars, top, method, input, GetUniquePrefix());
			ASTNode inputAST = parser.Parse();
			Assert.IsNotNull(inputAST, "Failed to parse the input");
			ASTNode expectedAST = ParseTree(unexpected);
			Assert.IsNotNull(expectedAST, "Failed to parse the unexpected tree");

			bool result = Compare(expectedAST, inputAST);
			Assert.IsFalse(result, "AST from input matches the unexpected AST, should not");
        }

		/// <summary>
        /// Tests whether the given grammar fails to parse the input as the expected AST
        /// </summary>
        /// <param name="grammars">Grammar content</param>
        /// <param name="top">The top grammar to compile</param>
        /// <param name="method">The parsing method to use</param>
        /// <param name="input">The input text to parse</param>
		protected void ParsingFails(string grammars, string top, ParsingMethod method, string input)
		{
			Hime.Redist.Parsers.IParser parser = BuildParser(grammars, top, method, input, GetUniquePrefix());
			parser.Parse();
			Assert.AreNotEqual(0, parser.Errors.Count, "Succeeded to parse the input");
		}
	}
}