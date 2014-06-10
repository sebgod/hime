/**********************************************************************
* Copyright (c) 2014 Laurent Wouters and others
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
using System.Collections.Generic;
using System.Xml;
using Hime.Redist;
using Hime.CentralDogma;
using Hime.CentralDogma.Input;
using Hime.CentralDogma.Output;

namespace Hime.Tests.Driver
{
	/// <summary>
	/// Represents a multi-platform test
	/// </summary>
	public class Test
	{
		public const string VERB_MATCHES = "matches";
		public const string VERB_NOMATCHES = "nomatches";
		public const string VERB_FAILS = "fails";
		public const string EXPECTED_PATH = "expected.xml";
		/// <summary>
		/// The test specification
		/// </summary>
		private ASTNode node;
		/// <summary>
		/// The original input for the test specification
		/// </summary>
		private Text originalInput;
		/// <summary>
		/// The verb for this test
		/// </summary>
		private string verb;
		/// <summary>
		/// The expected tree as XML
		/// </summary>
		private XmlDocument expected;
		/// <summary>
		/// The results per platform
		/// </summary>
		private Dictionary<Runtime, TestResult> results;

		/// <summary>
		/// Gets the test's name
		/// </summary>
		public string Name { get { return node.Children[0].Symbol.Value; } }

		/// <summary>
		/// Initializes this test
		/// </summary>
		/// <param name="node">The test specification</param>
		/// <param name="originalInput">The original input for the test specification</param>
		public Test(ASTNode node, Text originalInput)
		{
			this.node = node;
			this.originalInput = originalInput;
			switch (node.Symbol.Name)
			{
				case "test_matches":
					this.verb = VERB_MATCHES;
					break;
				case "test_no_match":
					this.verb = VERB_NOMATCHES;
					break;
				case "test_fails":
					this.verb = VERB_FAILS;
					break;
			}
			this.results = new Dictionary<Runtime, TestResult>();
			if (node.Children.Count >= 5)
			{
				this.expected = new XmlDocument();
				this.expected.AppendChild(this.expected.CreateXmlDeclaration("1.0", "utf-8", null));
				this.expected.AppendChild(GetExpectedAsXML(node.Children[4]));
			}
		}

		/// <summary>
		/// Gets the compilation unit for this test
		/// </summary>
		/// <returns>The compilation unit</returns>
		public Unit GetUnit(string fixture)
		{
			Loader loader = new Loader();
			loader.AddInput(node.Children[1], originalInput);
			return new Unit(
				loader.Load()[0],
				(ParsingMethod)Enum.Parse(typeof(ParsingMethod), node.Children[2].Symbol.Value),
				"Hime.Tests.Generated." + fixture,
				Modifier.Public);
		}

		/// <summary>
		/// Translates the specified AST node into the equivalent expected XML node
		/// </summary>
		/// <param name="node">An AST node</param>
		/// <returns>The expected XML node</returns>
		private XmlElement GetExpectedAsXML(ASTNode node)
		{
			XmlElement element = expected.CreateElement(node.Symbol.Value);
			if (node.Children[0].Children.Count != 0)
			{
				string test = node.Children[0].Children[0].Symbol.Value;
				string value = node.Children[0].Children[1].Symbol.Value;
				element.Attributes.Append(expected.CreateAttribute("test"));
				element.Attributes.Append(expected.CreateAttribute("value"));
				element.Attributes["test"].Value = (test == "=" ? VERB_MATCHES : VERB_NOMATCHES);
				element.Attributes["value"].Value = value.Substring(1, value.Length - 2);
			}
			foreach (ASTNode child in node.Children[1].Children)
				element.AppendChild(GetExpectedAsXML(child));
			return element;
		}

		/// <summary>
		/// Executes this test
		/// </summary>
		/// <param name="reporter">The reported to use</param>
		/// <param name="targets">The targets to execute on</param>
		/// <param name="fixture">The parent fixture's name</param>
		public void Execute(Reporter reporter, List<Runtime> targets, string fixture)
		{
			if (expected != null)
			{
				XmlWriter writer = new XmlTextWriter(EXPECTED_PATH, System.Text.Encoding.UTF8);
				expected.WriteTo(writer);
				writer.Close();
			}
			foreach (Runtime runtime in targets)
			{
				switch (runtime)
				{
					case Runtime.Net:
						this.results.Add(runtime, ExecuteOnNet(reporter, fixture));
						break;
					case Runtime.Java:
						this.results.Add(runtime, ExecuteOnJava(reporter, fixture));
						break;
				}
			}
		}

		/// <summary>
		/// Executes this test on the .Net runtime
		/// </summary>
		/// <param name="reporter">The reported to use</param>
		/// <param name="fixture">The parent fixture's name</param>
		/// <returns>The test result</returns>
		private TestResult ExecuteOnNet(Reporter reporter, string fixture)
		{
			TestResult result = new TestResult();
			List<string> output = new List<string>();
			int code = TestResult.RESULT_FAILURE_PARSING;
			try
			{
				System.Text.StringBuilder args = new System.Text.StringBuilder("executor.exe");
				// add parser name argument
				args.Append(" Hime.Tests.Generated.");
				args.Append(fixture);
				args.Append(".");
				args.Append(Name);
				args.Append("Parser");
				// add input argument
				args.Append(" ");
				args.Append(node.Children[3].Symbol.Value);
				// add verb argument
				args.Append(" ");
				args.Append(verb);
				code = ExecuteCommand(reporter, "mono", args.ToString(), output);
			}
			catch (Exception ex)
			{
				output.Add(ex.ToString());
			}
			result.Finish(code, output);
			return result;
		}

		/// <summary>
		/// Executes this test on the Java runtime
		/// </summary>
		/// <param name="reporter">The reported to use</param>
		/// <param name="fixture">The parent fixture's name</param>
		/// <returns>The test result</returns>
		private TestResult ExecuteOnJava(Reporter reporter, string fixture)
		{
			TestResult result = new TestResult();
			List<string> output = new List<string>();
			int code = TestResult.RESULT_FAILURE_PARSING;
			try
			{
				System.Text.StringBuilder args = new System.Text.StringBuilder("-jar executor.jar");
				// add parser name argument
				args.Append(" Hime.Tests.Generated.");
				args.Append(fixture);
				args.Append(".");
				args.Append(Name);
				args.Append("Parser");
				// add input argument
				args.Append(" ");
				args.Append(node.Children[3].Symbol.Value);
				// add verb argument
				args.Append(" ");
				args.Append(verb);
				code = ExecuteCommand(reporter, "java", args.ToString(), output);
			}
			catch (Exception ex)
			{
				output.Add(ex.ToString());
			}
			result.Finish(code, output);
			return result;
		}

		// <summary>
		/// Executes the specified command
		/// </summary>
		/// <param name="reporter">The reported to use</param>
		/// <param name="verb">The program to execute</param>
		/// <param name="arguments">The arguments</param>
		/// <param name="output">Storage for the console output lines</param>
		/// <returns>The command exit code</returns>
		private int ExecuteCommand(Reporter reporter, string command, string arguments, List<string> output)
		{
			reporter.Info("Executing command: " + command + " " + arguments);
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = command;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;
			process.Start();
			while (true)
			{
				string line = process.StandardOutput.ReadLine();
				if (line == null || line.Length == 0)
					break;
				output.Add(line);
				if (line.StartsWith("[ERROR]"))
					reporter.Error(line.Substring(8));
				else if (line.StartsWith("[WARNING]"))
					reporter.Warn(line.Substring(10));
				else if (line.StartsWith("[INFO]"))
					reporter.Info(line.Substring(7));
				else
					reporter.Info(line);
			}
			process.WaitForExit();
			return process.ExitCode;
		}

		/// <summary>
		/// Gets the XML report for this test
		/// </summary>
		/// <param name="doc">The parent XML document</param>
		/// <param name="fixture">The parent fixture's name</param>
		/// <returns>The XML report</returns>
		public ReportData GetXMLReport(XmlDocument doc, string fixture)
		{
			XmlElement root = doc.CreateElement("testsuite");
			root.Attributes.Append(doc.CreateAttribute("name"));
			root.Attributes.Append(doc.CreateAttribute("timestamp"));
			root.Attributes.Append(doc.CreateAttribute("tests"));
			root.Attributes.Append(doc.CreateAttribute("failures"));
			root.Attributes.Append(doc.CreateAttribute("errors"));
			root.Attributes.Append(doc.CreateAttribute("time"));

			ReportData aggregated = new ReportData();
			foreach (Runtime target in results.Keys)
			{
				ReportData data = GetXMLReport(doc, target, fixture);
				aggregated = aggregated + data;
				root.AppendChild(data.child);
			}
			aggregated.child = root;

			root.Attributes["name"].Value = fixture + "." + Name;
			root.Attributes["tests"].Value = results.Count.ToString();
			root.Attributes["failures"].Value = aggregated.failed.ToString();
			root.Attributes["errors"].Value = aggregated.errors.ToString();
			root.Attributes["time"].Value = aggregated.spent.ToString();

			return aggregated;
		}

		/// <summary>
		/// Gets the XML report for the specified target
		/// </summary>
		/// <param name="doc">The parent XML document</param>
		/// <param name="target">The target</param>
		/// <param name="fixture">The parent fixture's name</param>
		/// <returns>The XML report</returns>
		public ReportData GetXMLReport(XmlDocument doc, Runtime target, string fixture)
		{
			ReportData data = results[target].GetXML(doc);
			data.child.Attributes["name"].Value = target.ToString();
			data.child.Attributes["classname"].Value = fixture + "." + Name + "." + target.ToString();
			return data;
		}
	}
}