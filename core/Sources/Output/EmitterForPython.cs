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
using System.Collections.Generic;
using System.IO;

namespace Hime.CentralDogma.Output
{
	/// <summary>
	/// Represents an emitter of lexer and parser for a given grammar on the Python 3.3+ runtime
	/// </summary>
	public class EmitterForPython : EmitterBase
	{
		/// <summary>
		/// Gets the suffix for the emitted lexer code files
		/// </summary>
		public override string SuffixLexerCode { get { return "Lexer.py"; } }
		/// <summary>
		/// Gets suffix for the emitted parser code files
		/// </summary>
		public override string SuffixParserCode { get { return "Parser.py"; } }
		/// <summary>
		/// Gets suffix for the emitted assemblies
		/// </summary>
		public override string SuffixAssembly { get { return ".tar.gz"; } }

		/// <summary>
		/// Initializes this emitter
		/// </summary>
		/// <param name="units">The units to emit data for</param>
		public EmitterForPython(List<Unit> units) : base(new Reporter(), units)
		{
		}

		/// <summary>
		/// Initializes this emitter
		/// </summary>
		/// <param name="unit">The unit to emit data for</param>
		public EmitterForPython(Unit unit) : base(new Reporter(), unit)
		{
		}

		/// <summary>
		/// Initializes this emitter
		/// </summary>
		/// <param name="reporter">The reporter to use</param>
		/// <param name="units">The units to emit data for</param>
		public EmitterForPython(Reporter reporter, List<Unit> units) : base(reporter, units)
		{
		}

		/// <summary>
		/// Initializes this emitter
		/// </summary>
		/// <param name="reporter">The reporter to use</param>
		/// <param name="unit">The unit to emit data for</param>
		public EmitterForPython(Reporter reporter, Unit unit) : base(reporter, unit)
		{
		}

		/// <summary>
		/// Gets the runtime-specific generator of lexer code
		/// </summary>
		/// <param name="unit">The unit to generate a lexer for</param>
		/// <returns>The runtime-specific generator of lexer code</returns>
		protected override Generator GetLexerCodeGenerator(Unit unit)
		{
			return new LexerPythonCodeGenerator(unit, unit.Name + suffixLexerData);
		}

		/// <summary>
		/// Gets the runtime-specific generator of parser code
		/// </summary>
		/// <param name="unit">The unit to generate a parser for</param>
		/// <returns>The runtime-specific generator of parser code</returns>
		protected override Generator GetParserCodeGenerator(Unit unit)
		{
			return new ParserNetCodeGenerator(unit, unit.Name + suffixParserData);
		}

		/// <summary>
		/// Emits the assembly for the generated lexer and parser
		/// </summary>
		/// <returns><c>true</c> if the operation succeed</returns>
		protected override bool EmitAssembly()
		{
			throw new NotImplementedException();
		}
	}
}