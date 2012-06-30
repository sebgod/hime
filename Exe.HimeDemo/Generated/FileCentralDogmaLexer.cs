﻿/*
 * WARNING: this file has been generated by
 * Hime Parser Generator 0.4.0.0
 */

using System.Collections.Generic;
using Hime.Redist.Parsers;

namespace Hime.Demo.Generated.CD
{
    public class FileCentralDogmaLexer : TextLexer
    {
        private static readonly TextLexerAutomaton automaton = TextLexerAutomaton.FindAutomaton(typeof(FileCentralDogmaLexer));
        public static readonly SymbolTerminal[] terminals = {
            new SymbolTerminal(0x1, "ε"),
            new SymbolTerminal(0x2, "$"),
            new SymbolTerminal(0x60, "["),
            new SymbolTerminal(0xB, "INTEGER"),
            new SymbolTerminal(0x14, "="),
            new SymbolTerminal(0x15, ";"),
            new SymbolTerminal(0x16, "."),
            new SymbolTerminal(0x18, "("),
            new SymbolTerminal(0x19, ")"),
            new SymbolTerminal(0x1A, "*"),
            new SymbolTerminal(0x1B, "+"),
            new SymbolTerminal(0x1C, "?"),
            new SymbolTerminal(0x1D, "{"),
            new SymbolTerminal(0x1E, ","),
            new SymbolTerminal(0x1F, "}"),
            new SymbolTerminal(0x20, "-"),
            new SymbolTerminal(0x21, "|"),
            new SymbolTerminal(0x24, "<"),
            new SymbolTerminal(0x25, ">"),
            new SymbolTerminal(0x26, "^"),
            new SymbolTerminal(0x27, "!"),
            new SymbolTerminal(0xA, "NAME"),
            new SymbolTerminal(0x2B, ":"),
            new SymbolTerminal(0x61, "]"),
            new SymbolTerminal(0x7, "SEPARATOR"),
            new SymbolTerminal(0xC, "QUOTED_DATA"),
            new SymbolTerminal(0xD, "ESCAPEES"),
            new SymbolTerminal(0x22, "=>"),
            new SymbolTerminal(0x17, ".."),
            new SymbolTerminal(0x23, "->"),
            new SymbolTerminal(0x2C, "cf"),
            new SymbolTerminal(0x62, "cs"),
            new SymbolTerminal(0xE, "SYMBOL_TERMINAL_TEXT"),
            new SymbolTerminal(0xF, "SYMBOL_TERMINAL_SET"),
            new SymbolTerminal(0x12, "SYMBOL_VALUE_UINT8"),
            new SymbolTerminal(0x2A, "rules"),
            new SymbolTerminal(0x10, "SYMBOL_TERMINAL_UBLOCK"),
            new SymbolTerminal(0x11, "SYMBOL_TERMINAL_UCAT"),
            new SymbolTerminal(0x13, "SYMBOL_VALUE_UINT16"),
            new SymbolTerminal(0x28, "options"),
            new SymbolTerminal(0x2D, "grammar"),
            new SymbolTerminal(0x29, "terminals") };
        public FileCentralDogmaLexer(string input) : base(automaton, terminals, 0x7, new System.IO.StringReader(input)) {}
        public FileCentralDogmaLexer(System.IO.TextReader input) : base(automaton, terminals, 0x7, input) {}
    }
}
