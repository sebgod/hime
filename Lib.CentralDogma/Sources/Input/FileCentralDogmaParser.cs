﻿/*
 * WARNING: this file has been generated by
 * Hime Parser Generator 0.6.0.0
 */

using System.Collections.Generic;
using Hime.Redist.Symbols;
using Hime.Redist.Lexer;
using Hime.Redist.Parsers;

namespace Hime.CentralDogma.Input
{
    internal class FileCentralDogmaParser : LRkParser
    {
        private static readonly LRkAutomaton automaton = LRkAutomaton.FindAutomaton(typeof(FileCentralDogmaParser), "FileCentralDogmaParser.bin");
        private static readonly Variable[] variables = {
            new Variable(0x2F, "option"), 
            new Variable(0x30, "terminal_def_atom_any"), 
            new Variable(0x31, "terminal_def_atom_unicode"), 
            new Variable(0x32, "terminal_def_atom_text"), 
            new Variable(0x33, "terminal_def_atom_set"), 
            new Variable(0x34, "terminal_def_atom_ublock"), 
            new Variable(0x35, "terminal_def_atom_ucat"), 
            new Variable(0x36, "terminal_def_atom_span"), 
            new Variable(0x37, "terminal_def_atom"), 
            new Variable(0x38, "terminal_def_element"), 
            new Variable(0x39, "terminal_def_cardinalilty"), 
            new Variable(0x3A, "terminal_def_repetition"), 
            new Variable(0x3B, "terminal_def_fragment"), 
            new Variable(0x3C, "terminal_def_restrict"), 
            new Variable(0x3D, "terminal_definition"), 
            new Variable(0x3E, "terminal_subgrammar"), 
            new Variable(0x3F, "terminal"), 
            new Variable(0x40, "rule_sym_action"), 
            new Variable(0x41, "rule_sym_virtual"), 
            new Variable(0x42, "rule_sym_ref_params"), 
            new Variable(0x43, "rule_sym_ref_template"), 
            new Variable(0x44, "rule_sym_ref_simple"), 
            new Variable(0x45, "rule_def_atom"), 
            new Variable(0x46, "rule_def_element"), 
            new Variable(0x47, "rule_def_tree_action"), 
            new Variable(0x48, "rule_def_repetition"), 
            new Variable(0x49, "rule_def_fragment"), 
            new Variable(0x4A, "rule_def_restrict"), 
            new Variable(0x4B, "rule_def_choice"), 
            new Variable(0x4C, "rule_definition"), 
            new Variable(0x4D, "rule_template_params"), 
            new Variable(0x4E, "cf_rule_template"), 
            new Variable(0x4F, "cf_rule_simple"), 
            new Variable(0x50, "grammar_options"), 
            new Variable(0x51, "grammar_terminals"), 
            new Variable(0x52, "grammar_cf_rules"), 
            new Variable(0x53, "grammar_parency"), 
            new Variable(0x54, "cf_grammar"), 
            new Variable(0x55, "_v9370D0A"), 
            new Variable(0x56, "_v4D8E02AC"), 
            new Variable(0x57, "_vF395BF6"), 
            new Variable(0x58, "_v371DCAF6"), 
            new Variable(0x59, "_v59831D94"), 
            new Variable(0x5A, "_v480246D5"), 
            new Variable(0x5B, "_v5DE22695"), 
            new Variable(0x5C, "_v567743EB"), 
            new Variable(0x5D, "_v517B684B"), 
            new Variable(0x5E, "_v5C6871D"), 
            new Variable(0x5F, "_v2E2216FF"), 
            new Variable(0x60, "_v41D31CF7"), 
            new Variable(0x64, "cs_rule_context"), 
            new Variable(0x65, "cs_rule_template"), 
            new Variable(0x66, "cs_rule_simple"), 
            new Variable(0x67, "grammar_cs_rules"), 
            new Variable(0x68, "cs_grammar"), 
            new Variable(0x69, "_v6A84ACEF"), 
            new Variable(0x6A, "file_item"), 
            new Variable(0x6B, "file"), 
            new Variable(0x6C, "_v44FDD778"), 
            new Variable(0x6D, "_Axiom_") };
        private static readonly Virtual[] virtuals = {
            new Virtual("range"), 
            new Virtual("concat"), 
            new Virtual("emptypart") };
        public FileCentralDogmaParser(FileCentralDogmaLexer lexer) : base (automaton, variables, virtuals, null, lexer) { }
    }
}
