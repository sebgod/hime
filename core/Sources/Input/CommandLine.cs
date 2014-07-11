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
using System.Text;
using Hime.Redist;

namespace Hime.CentralDogma.Input
{
	/// <summary>
	/// Contains utilities for the manipulation of command line arguments
	/// </summary>
	public class CommandLine
	{
		/// <summary>
		/// Parses the command line arguments
		/// </summary>
		/// <param name="args">The command line arguments</param>
		/// <returns>The parsed line as an AST, or null if the parsing failed</returns>
		public static ParseResult ParseArguments(string[] args)
		{
			StringBuilder builder = new StringBuilder();
			foreach (string arg in args)
			{
				builder.Append(" ");
				builder.Append(arg);
			}
			CommandLineLexer lexer = new CommandLineLexer(builder.ToString());
			CommandLineParser parser = new CommandLineParser(lexer);
			ParseResult result = parser.Parse();
			foreach (ParseError error in result.Errors)
				Console.WriteLine(error.Message);
			return result;
		}
	}
}