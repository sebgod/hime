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
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Hime.Redist;
using Hime.Redist.Parsers;
using Hime.Redist.Utils;
using Hime.SDK;
using Hime.SDK.Output;
using Hime.SDK.Reflection;

namespace Hime.Tests.Driver
{
	/// <summary>
	/// Represents a multi-platform test fixture
	/// </summary>
	public class Fixture
	{
		/// <summary>
		/// The assembly for the fixture parser
		/// </summary>
		private static AssemblyReflection parserFixture = BuildFixtureParser();

		/// <summary>
		/// Builds the fixture parser
		/// </summary>
		/// <returns>The fixture parser assembly</returns>
		private static AssemblyReflection BuildFixtureParser()
		{
			Stream stream1 = typeof(Program).Assembly.GetManifestResourceStream("Hime.Tests.Driver.Resources.Fixture.gram");
			Stream stream2 = typeof(CompilationTask).Assembly.GetManifestResourceStream("Hime.SDK.Sources.Input.HimeGrammar.gram");
			CompilationTask task = new CompilationTask();
			task.AddInputRaw(stream1);
			task.AddInputRaw(stream2);
			task.GrammarName = "Fixture";
			task.CodeAccess = Modifier.Public;
			task.Method = ParsingMethod.LALR1;
			task.Mode = Mode.Assembly;
			task.Namespace = "Hime.Tests.Driver";
			task.Execute();
			return new AssemblyReflection("Fixture.dll");
		}

		/// <summary>
		/// The fixture's name
		/// </summary>
		private readonly string name;
		/// <summary>
		/// The contained tests
		/// </summary>
		private readonly List<Test> tests;

		/// <summary>
		/// Gets the fixture's name
		/// </summary>
		public string Name { get { return name; } }

		/// <summary>
		/// Gets the tests in this fixture
		/// </summary>
		public ROList<Test> Tests { get { return new ROList<Test>(tests); } }

		/// <summary>
		/// Loads this fixture
		/// </summary>
		/// <param name="reporter">The reported to use</param>
		/// <param name="name">The fixture's name</param>
		public Fixture(Reporter reporter, string name)
		{
			reporter.Info("Loading fixture " + name);
			Stream stream = typeof(Program).Assembly.GetManifestResourceStream(name);
			TextReader reader = new StreamReader(stream, Encoding.UTF8);
			string content = reader.ReadToEnd();
			reader.Close();
			BaseLRParser parser = parserFixture.GetParser(content);
			ParseResult result = parser.Parse();
			foreach (ParseError error in result.Errors)
				reporter.Error(error, result.Input, error.Position);
			ASTNode fixtureNode = result.Root;
			this.name = fixtureNode.Value;
			tests = new List<Test>();
			foreach (ASTNode testNode in fixtureNode.Children)
			{
				Test test;
				switch (testNode.Symbol.Name)
				{
					case "test_output":
						test = new OutputTest(testNode, result.Input);
						break;
					default:
						test = new ParsingTest(testNode, result.Input);
						break;
				}
				tests.Add(test);
			}
		}

		/// <summary>
		/// Executes this fixture
		/// </summary>
		/// <param name="reporter">The reported to use</param>
		public void Execute(Reporter reporter)
		{
			foreach (Test test in tests)
				test.Execute(reporter, name);
		}

		/// <summary>
		/// Gets the XML report for this fixture
		/// </summary>
		/// <param name="doc">The parent XML document</param>
		/// <returns>The XML report</returns>
		public ReportData GetXMLReport(XmlDocument doc)
		{
			XmlElement root = doc.CreateElement("testsuite");
			root.Attributes.Append(doc.CreateAttribute("name"));
			root.Attributes.Append(doc.CreateAttribute("timestamp"));
			root.Attributes.Append(doc.CreateAttribute("tests"));
			root.Attributes.Append(doc.CreateAttribute("failures"));
			root.Attributes.Append(doc.CreateAttribute("errors"));
			root.Attributes.Append(doc.CreateAttribute("time"));

			ReportData aggregated = new ReportData();
			foreach (Test test in tests)
			{
				ReportData data = test.GetXMLReport(doc, name);
				aggregated = aggregated + data;
				root.AppendChild(data.child);
			}
			aggregated.child = root;

			root.Attributes["name"].Value = name;
			root.Attributes["tests"].Value = (aggregated.passed + aggregated.errors + aggregated.failed).ToString();
			root.Attributes["failures"].Value = aggregated.failed.ToString();
			root.Attributes["errors"].Value = aggregated.errors.ToString();
			root.Attributes["time"].Value = aggregated.spent.TotalSeconds.ToString(CultureInfo.InvariantCulture);

			return aggregated;
		}
	}
}