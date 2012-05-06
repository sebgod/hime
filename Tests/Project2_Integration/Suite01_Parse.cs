﻿/*
 * Author: Charles Hymans
 * */
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using NUnit.Framework;
using Hime.Parsers;
using Hime.Utils.Reporting;
using Hime.Redist.Parsers;
using System.IO;
using System.CodeDom.Compiler;

namespace Hime.Tests.Project2_Integration
{
    [TestFixture]
    public class Suite01_Parse : BaseTestSuite
    {
        private const string grammar0 = "cf grammar Test { options{ Axiom=\"S\"; } terminals{} rules{ S->'a'S'b'T|'c'T|'d'; T->'a'T|'b'S|'c'; } }";
        private const string grammar1 = "cf grammar Test { options{ Axiom=\"test\"; } terminals{} rules{ test->'x'*; } }";

        private void TestGrammar(string grammar, ParsingMethod method, string input)
        {
            Assert.IsFalse(CompileRaw(grammar, method).HasErrors, "Grammar compilation failed!");
            Assembly assembly = Build();
            bool errors = false;
            SyntaxTreeNode node = Parse(assembly, input, out errors);
            Assert.NotNull(node, "Failed to parse input!");
            Assert.IsFalse(errors, "Parsing errors!");
        }

        [Test]
        public void Test001_SimpleGrammar_LR0()
        {
			CompileRaw("cf grammar Test { options{ Axiom=\"S\"; } terminals{} rules{ S->'a'; } }", ParsingMethod.LR0);
            Build();
        }

		[Test]
        public void Test002_SimpleGrammar_LR0()
        {
            TestGrammar(grammar0, ParsingMethod.LR0, "adbc");
        }

        [Test]
        public void Test003_Build_ShouldNotFail()
        {
			CompileRaw(grammar1, ParsingMethod.LR1);
			Build();
		}
		
		[Test]
        public void Test004_SimpleList_LR1()
        {
            TestGrammar(grammar1, ParsingMethod.LR1, "xxx");
        }

        [Test]
        public void Test005_SimpleList_LALR1()
        {
            TestGrammar(grammar1, ParsingMethod.LALR1, "xxx");
        }

        [Test]
        public void Test006_LRStar_ShouldNotFail()
        {
			Report result = CompileRaw(grammar1, ParsingMethod.LRStar);
            Assert.IsFalse(result.HasErrors);
        }

		[Test]
        public void Test007_SimpleList_LRStar()
        {
            TestGrammar(grammar1, ParsingMethod.LRStar, "xxx");
        }
    }
}