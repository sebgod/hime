﻿/*
 * WARNING: this file has been generated by
 * Hime Parser Generator 0.4.0.0
 */

using System.Collections.Generic;
using Hime.Redist.Parsers;
using Hime.Redist.Symbols;

namespace Hime.Benchmark.Generated.CD
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
            new Variable(0x55, "_v5A30933E"), 
            new Variable(0x56, "_v1F439E75"), 
            new Variable(0x57, "_v72874907"), 
            new Variable(0x58, "_v4346BDB8"), 
            new Variable(0x59, "_v334E3E27"), 
            new Variable(0x5A, "_v6C467146"), 
            new Variable(0x5B, "_v41E9A29E"), 
            new Variable(0x5C, "_v672C2F9B"), 
            new Variable(0x5D, "_v25ED9760"), 
            new Variable(0x5E, "_v371D3870"), 
            new Variable(0x5F, "_v1A1F2BA8"), 
            new Variable(0x60, "_v358CB530"), 
            new Variable(0x64, "cs_rule_context"), 
            new Variable(0x65, "cs_rule_template"), 
            new Variable(0x66, "cs_rule_simple"), 
            new Variable(0x67, "grammar_cs_rules"), 
            new Variable(0x68, "cs_grammar"), 
            new Variable(0x69, "_v4DB3AF75"), 
            new Variable(0x6A, "file_item"), 
            new Variable(0x6B, "file"), 
            new Variable(0x6C, "_v56736446"), 
            new Variable(0x6D, "_Axiom_") };
        private static readonly Virtual[] virtuals = {
            new Virtual("range"), 
            new Virtual("concat"), 
            new Virtual("emptypart") };
        public FileCentralDogmaParser(FileCentralDogmaLexer lexer) : base (automaton, variables, virtuals, null, lexer) { }
    }
}
