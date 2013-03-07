﻿/*
 * WARNING: this file has been generated by
 * Hime Parser Generator 0.4.0.0
 */

using System.Collections.Generic;
using Hime.Redist.Symbols;
using Hime.Redist.Lexer;
using Hime.Redist.Parsers;

namespace Hime.Demo.Generated
{
    internal class MathExpParser : LRkParser
    {
        private static readonly LRkAutomaton automaton = LRkAutomaton.FindAutomaton(typeof(MathExpParser), "MathExpParser.bin");
        private static readonly Variable[] variables = {
            new Variable(0x8, "exp_atom"), 
            new Variable(0x9, "exp_op0"), 
            new Variable(0xA, "exp_op1"), 
            new Variable(0xB, "exp"), 
            new Variable(0x12, "_Axiom_") };
        private static readonly Virtual[] virtuals = {
 };
        public sealed class Actions
        {
            private void Null(Variable head, Symbol[] body, int length) { }
            private SemanticAction nullAction;
            public SemanticAction NullAction { get { return nullAction; } }
            private SemanticAction[] raw;
            public SemanticAction[] RawActions { get { return raw; } }
            public Actions()
            {
                nullAction = new SemanticAction(Null);
                raw = new SemanticAction[5];
                raw[0] = nullAction;
                raw[1] = nullAction;
                raw[2] = nullAction;
                raw[3] = nullAction;
                raw[4] = nullAction;
            }
            public SemanticAction OnNumber
            {
                get { return raw[0]; }
                set { raw[0] = value; }
            }
            public SemanticAction OnMult
            {
                get { return raw[1]; }
                set { raw[1] = value; }
            }
            public SemanticAction OnDiv
            {
                get { return raw[2]; }
                set { raw[2] = value; }
            }
            public SemanticAction OnPlus
            {
                get { return raw[3]; }
                set { raw[3] = value; }
            }
            public SemanticAction OnMinus
            {
                get { return raw[4]; }
                set { raw[4] = value; }
            }
        }
        public MathExpParser(MathExpLexer lexer, Actions actions) : base (automaton, variables, virtuals, actions.RawActions, lexer) { }
    }
}
