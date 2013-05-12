﻿/*
 * WARNING: this file has been generated by
 * Hime Parser Generator 0.6.0.0
 */

using System.Collections.Generic;
using Hime.Redist.Symbols;
using Hime.Redist.Lexer;
using Hime.Redist.Parsers;

namespace Hime.HimeCC.CL
{
    internal class CommandLineParser : LRkParser
    {
        private static readonly LRkAutomaton automaton = LRkAutomaton.FindAutomaton(typeof(CommandLineParser), "CommandLineParser.bin");
        private static readonly Variable[] variables = {
            new Variable(0x9, "value"), 
            new Variable(0xA, "argument"), 
            new Variable(0xB, "values"), 
            new Variable(0xC, "arguments"), 
            new Variable(0xD, "line"), 
            new Variable(0xE, "_v43A72D99"), 
            new Variable(0xF, "_v2EA3EB72"), 
            new Variable(0x10, "_v65207E07"), 
            new Variable(0x11, "_Axiom_") };
        private static readonly Virtual[] virtuals = {
 };
        public CommandLineParser(CommandLineLexer lexer) : base (automaton, variables, virtuals, null, lexer) { }
    }
}