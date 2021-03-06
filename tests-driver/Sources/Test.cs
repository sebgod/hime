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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.Xml;
using Hime.SDK;
using Hime.SDK.Output;

namespace Hime.Tests.Driver
{
	/// <summary>
	/// Represents a multi-platform test
	/// </summary>
	public abstract class Test
	{
		/// <summary>
		/// The results per platform
		/// </summary>
		protected Dictionary<Runtime, TestResult> results;

		/// <summary>
		/// Gets the test's name
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Initializes the test
		/// </summary>
		protected Test()
		{
			results = new Dictionary<Runtime, TestResult>();
		}

		/// <summary>
		/// Gets the compilation unit for this test
		/// </summary>
		/// <returns>The compilation unit</returns>
		public abstract Unit GetUnit(string fixture);

		/// <summary>
		/// Executes this test
		/// </summary>
		/// <param name="reporter">The reported to use</param>
		/// <param name="fixture">The parent fixture's name</param>
		public abstract void Execute(Reporter reporter, string fixture);

		/// <summary>
		/// Executes the specified command
		/// </summary>
		/// <param name="reporter">The reported to use</param>
		/// <param name="command">The command to execute</param>
		/// <param name="arguments">The arguments</param>
		/// <param name="output">Storage for the console output lines</param>
		/// <returns>The command exit code</returns>
		protected int ExecuteCommand(Reporter reporter, string command, string arguments, List<string> output)
		{
			reporter.Info("Executing command: " + command + " " + arguments);
			Process process = new Process();
			// inject useful environment variables
			process.StartInfo.EnvironmentVariables.Add("RUST_BACKTRACE", "1");
			process.StartInfo.FileName = command;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.Start();
			while (true)
			{
				string line = process.StandardOutput.ReadLine();
				if (string.IsNullOrEmpty(line))
					break;
				output.Add(line);
				reporter.Info(line);
			}
			while (true)
			{
				string line = process.StandardError.ReadLine();
				if (string.IsNullOrEmpty(line))
					break;
				output.Add(line);
				reporter.Error(line);
			}
			process.WaitForExit();
			int code = process.ExitCode;
			process.Close();
			return code;
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
			root.Attributes["time"].Value = aggregated.spent.TotalSeconds.ToString(CultureInfo.InvariantCulture);

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
			data.child.Attributes["classname"].Value = fixture + "." + Name;
			return data;
		}
	}
}