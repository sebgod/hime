﻿/*
 * Author: Charles Hymans
 * Date: 18/07/2011
 * Time: 20:59
 * 
 */
using System;
using NUnit.Framework;
using Hime.Parsers;
using Hime.Utils.Reporting;
using System.IO;

namespace Hime.Tests.Project0_CentralDogma
{
	[TestFixture]
	public class Suite05_CompilationTask
	{
		[Test]
        public void Test000_ExecuteBody_LR0_ShouldNotFailOnSimpleGrammar()
        {
        	string grammar = 
        		"cf grammar Test { options { Axiom=\"exp\"; } terminals { } rules { exp -> 'x'; } }";
            CompilationTask task = new CompilationTask();
            task.Method = ParsingMethod.LR0;
            task.InputRawData.Add(grammar);
			task.GrammarName = "Test";
            task.ExecuteDo();
        }

        [Test]
        public void Test001_ExecuteLoadData_ShouldNotHaveAnyErrorWhenSectionTerminalsIsNotPresent()
        {
        	string grammar = 
        		"cf grammar Test { options { Axiom=\"exp\"; } rules { exp -> 'x'; } }";
            CompilationTask task = new CompilationTask();
            task.Method = ParsingMethod.LR0;
            task.InputRawData.Add(grammar);
			task.GrammarName = "Test";
            Assert.IsFalse(task.LoadInputs());
        }
        
        // should stop earlier on error of the lexer => do a test with an syntax error in the input. See if it stops early
        [Test]
        public void Test002_Execute_ShouldNotHaveAnyErrorWhenSectionTerminalsIsNotPresent()
        {
        	string grammar = 
        		"cf grammar Test { options { Axiom=\"exp\"; } rules { exp -> 'x'; } }";
            CompilationTask task = new CompilationTask();
            task.Method = ParsingMethod.LR0;
            task.InputRawData.Add(grammar);
			task.GrammarName = "Test";
            Report result = task.Execute();
            Assert.IsFalse(result.HasErrors);
        }
        
        [Test]
        public void Test003_Execute_LexerFileShouldBeGrammarNameSuffixLexer_Issue198()
        {
        	string grammar = 
        		"cf grammar Test { options { Axiom=\"exp\"; } rules { exp -> 'x'; } }";
            CompilationTask task = new CompilationTask();
            task.Method = ParsingMethod.LR0;
            task.InputRawData.Add(grammar);
			task.GrammarName = "Test";
            task.Execute();
            Assert.IsTrue(File.Exists("TestLexer.cs"));
        }
        
        [Test]
        public void Test004_Execute_ParserFileShouldBeGrammarNameSuffixLexer_Issue198()
        {
        	string grammar = 
        		"cf grammar Test { options { Axiom=\"exp\"; } rules { exp -> 'x'; } }";
            CompilationTask task = new CompilationTask();
            task.Method = ParsingMethod.LR0;
            task.InputRawData.Add(grammar);
			task.GrammarName = "Test";
            task.Execute();
            Assert.IsTrue(File.Exists("TestParser.cs"));
        }
	}
}
