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
using Hime.Redist;
using Hime.SDK.Input;

namespace Hime.Benchmark
{
	/// <summary>
	/// Main program class
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Program's entry point
		/// </summary>
		/// <param name="args">Command line arguments</param>
		public static int Main(string[] args)
		{
			// If no argument is given, print the help screen and return OK
			if (args == null || args.Length == 0)
			{
				PrintHelp();
				return 0;
			}

			// Parse the arguments
			ParseResult result = CommandLine.ParseArguments(args);
			if (!result.IsSuccess || result.Errors.Count > 0)
			{
				foreach (ParseError error in result.Errors)
				{
					TextContext context = result.Input.GetContext(error.Position);
					Console.WriteLine(error.Message);
					Console.WriteLine("\t" + context.Content);
					Console.WriteLine("\t" + context.Pointer);
				}
				return 1;
			}

			ASTNode line = result.Root;
			ASTNode inputs = line.Children[0];
			if (inputs.Children.Count < 2)
			{
				PrintHelp();
				return 1;
			}

			Task task = null;
			string assemblyPath = inputs.Children[0].Value;
			string inputPath = inputs.Children[1].Value;
			bool useStream = false;
			foreach (ASTNode arg in line.Children[1].Children)
			{
				switch (arg.Value)
				{
					case "--stream":
						useStream = true;
						break;
					case "--lexer":
						task = new BenchmarkLexer();
						break;
					case "--stats":
						task = new InputStats();
						break;
				}
			}
			if (task == null)
				task = new BenchmarkParser();

			task.Execute(assemblyPath, inputPath, useStream);
			return 0;
		}

		/// <summary>
		/// Prints the help screen for this program
		/// </summary>
		private static void PrintHelp()
		{
			Console.WriteLine("Hime.Benchmark " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " (LGPL 3)");
			Console.WriteLine("Benchmarking facility for the Hime parsers");
			Console.WriteLine();
			Console.WriteLine("usage: Hime.Benchmark <assembly> <input> [options]");
			Console.WriteLine("options:");
			Console.WriteLine("--stream\tUses a stream to read from the input");
			Console.WriteLine("--lexer\tBenchmark the lexer alone");
		}
	}
}