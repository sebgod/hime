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
using Hime.CentralDogma;
using Hime.Redist.AST;
using Hime.Redist.Parsers;
using NUnit.Framework;

namespace Hime.Tests
{
	/// <summary>
	/// Integration test suite for high-level parsing features
	/// </summary>
    [TestFixture]
    public class HimeIntegration : BaseParseSuite
    {
		/// <summary>
		/// Tests the correct regeneration of the Central Dogma input parser
		/// </summary>
		[Test]
		public void Test_CentralDogma_Regeneration()
		{
			SetTestDirectory();

			Stream stream = typeof(CompilationTask).Assembly.GetManifestResourceStream("Hime.CentralDogma.Sources.Input.FileCentralDogma.gram");
			StreamReader reader = new StreamReader(stream);
			string grammar = reader.ReadToEnd();

			IParser parser = BuildParser(grammar, "FileCentralDogma", ParsingMethod.LALR1, grammar, "Test_CentralDogma_Regeneration");
			Assert.IsNotNull(parser, "Failed to compile the parser");
			ASTNode root = parser.Parse();
			Assert.IsNotNull(root, "Failed to parse the Central Dogma grammar with the generated parser");
			Assert.AreEqual(0, parser.Errors.Count, "Some error while parsing the Central Dogma grammar with the generated parser");
		}
    }
}
