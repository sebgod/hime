/*
 * WARNING: this file has been generated by
 * Hime Parser Generator 3.3.1.0
 */
using System.Collections.Generic;
using Hime.Redist;
using Hime.Redist.Parsers;

namespace Hime.SDK.Input
{
	/// <summary>
	/// Represents a parser
	/// </summary>
	internal class HimeGrammarParser : LRkParser
	{
		/// <summary>
		/// The automaton for this parser
		/// </summary>
		private static readonly LRkAutomaton commonAutomaton = LRkAutomaton.Find(typeof(HimeGrammarParser), "HimeGrammarParser.bin");
		/// <summary>
		/// Contains the constant IDs for the variables and virtuals in this parser
		/// </summary>
		public class ID
		{
			/// <summary>
			/// The unique identifier for variable option
			/// </summary>
			public const int VariableOption = 0x001F;
			/// <summary>
			/// The unique identifier for variable terminal_def_atom
			/// </summary>
			public const int VariableTerminalDefAtom = 0x0020;
			/// <summary>
			/// The unique identifier for variable terminal_def_element
			/// </summary>
			public const int VariableTerminalDefElement = 0x0021;
			/// <summary>
			/// The unique identifier for variable terminal_def_cardinalilty
			/// </summary>
			public const int VariableTerminalDefCardinalilty = 0x0022;
			/// <summary>
			/// The unique identifier for variable terminal_def_repetition
			/// </summary>
			public const int VariableTerminalDefRepetition = 0x0023;
			/// <summary>
			/// The unique identifier for variable terminal_def_fragment
			/// </summary>
			public const int VariableTerminalDefFragment = 0x0024;
			/// <summary>
			/// The unique identifier for variable terminal_def_restrict
			/// </summary>
			public const int VariableTerminalDefRestrict = 0x0025;
			/// <summary>
			/// The unique identifier for variable terminal_definition
			/// </summary>
			public const int VariableTerminalDefinition = 0x0026;
			/// <summary>
			/// The unique identifier for variable terminal_rule
			/// </summary>
			public const int VariableTerminalRule = 0x0027;
			/// <summary>
			/// The unique identifier for variable terminal_fragment
			/// </summary>
			public const int VariableTerminalFragment = 0x0028;
			/// <summary>
			/// The unique identifier for variable terminal_context
			/// </summary>
			public const int VariableTerminalContext = 0x0029;
			/// <summary>
			/// The unique identifier for variable terminal_item
			/// </summary>
			public const int VariableTerminalItem = 0x002A;
			/// <summary>
			/// The unique identifier for variable rule_sym_action
			/// </summary>
			public const int VariableRuleSymAction = 0x002B;
			/// <summary>
			/// The unique identifier for variable rule_sym_virtual
			/// </summary>
			public const int VariableRuleSymVirtual = 0x002C;
			/// <summary>
			/// The unique identifier for variable rule_sym_ref_params
			/// </summary>
			public const int VariableRuleSymRefParams = 0x002D;
			/// <summary>
			/// The unique identifier for variable rule_sym_ref_template
			/// </summary>
			public const int VariableRuleSymRefTemplate = 0x002E;
			/// <summary>
			/// The unique identifier for variable rule_sym_ref_simple
			/// </summary>
			public const int VariableRuleSymRefSimple = 0x002F;
			/// <summary>
			/// The unique identifier for variable rule_def_atom
			/// </summary>
			public const int VariableRuleDefAtom = 0x0030;
			/// <summary>
			/// The unique identifier for variable rule_def_context
			/// </summary>
			public const int VariableRuleDefContext = 0x0031;
			/// <summary>
			/// The unique identifier for variable rule_def_element
			/// </summary>
			public const int VariableRuleDefElement = 0x0032;
			/// <summary>
			/// The unique identifier for variable rule_def_tree_action
			/// </summary>
			public const int VariableRuleDefTreeAction = 0x0033;
			/// <summary>
			/// The unique identifier for variable rule_def_repetition
			/// </summary>
			public const int VariableRuleDefRepetition = 0x0034;
			/// <summary>
			/// The unique identifier for variable rule_def_fragment
			/// </summary>
			public const int VariableRuleDefFragment = 0x0035;
			/// <summary>
			/// The unique identifier for variable rule_def_choice
			/// </summary>
			public const int VariableRuleDefChoice = 0x0036;
			/// <summary>
			/// The unique identifier for variable rule_definition
			/// </summary>
			public const int VariableRuleDefinition = 0x0037;
			/// <summary>
			/// The unique identifier for variable rule_template_params
			/// </summary>
			public const int VariableRuleTemplateParams = 0x0038;
			/// <summary>
			/// The unique identifier for variable cf_rule_template
			/// </summary>
			public const int VariableCfRuleTemplate = 0x0039;
			/// <summary>
			/// The unique identifier for variable cf_rule_simple
			/// </summary>
			public const int VariableCfRuleSimple = 0x003A;
			/// <summary>
			/// The unique identifier for variable cf_rule
			/// </summary>
			public const int VariableCfRule = 0x003B;
			/// <summary>
			/// The unique identifier for variable grammar_options
			/// </summary>
			public const int VariableGrammarOptions = 0x003C;
			/// <summary>
			/// The unique identifier for variable grammar_terminals
			/// </summary>
			public const int VariableGrammarTerminals = 0x003D;
			/// <summary>
			/// The unique identifier for variable grammar_cf_rules
			/// </summary>
			public const int VariableGrammarCfRules = 0x003E;
			/// <summary>
			/// The unique identifier for variable grammar_parency
			/// </summary>
			public const int VariableGrammarParency = 0x003F;
			/// <summary>
			/// The unique identifier for variable cf_grammar
			/// </summary>
			public const int VariableCfGrammar = 0x0040;
			/// <summary>
			/// The unique identifier for variable file
			/// </summary>
			public const int VariableFile = 0x0041;
			/// <summary>
			/// The unique identifier for virtual range
			/// </summary>
			public const int VirtualRange = 0x0046;
			/// <summary>
			/// The unique identifier for virtual concat
			/// </summary>
			public const int VirtualConcat = 0x004A;
			/// <summary>
			/// The unique identifier for virtual emptypart
			/// </summary>
			public const int VirtualEmptypart = 0x0057;
		}
		/// <summary>
		/// The collection of variables matched by this parser
		/// </summary>
		/// <remarks>
		/// The variables are in an order consistent with the automaton,
		/// so that variable indices in the automaton can be used to retrieve the variables in this table
		/// </remarks>
		private static readonly Symbol[] variables = {
			new Symbol(0x001F, "option"), 
			new Symbol(0x0020, "terminal_def_atom"), 
			new Symbol(0x0021, "terminal_def_element"), 
			new Symbol(0x0022, "terminal_def_cardinalilty"), 
			new Symbol(0x0023, "terminal_def_repetition"), 
			new Symbol(0x0024, "terminal_def_fragment"), 
			new Symbol(0x0025, "terminal_def_restrict"), 
			new Symbol(0x0026, "terminal_definition"), 
			new Symbol(0x0027, "terminal_rule"), 
			new Symbol(0x0028, "terminal_fragment"), 
			new Symbol(0x0029, "terminal_context"), 
			new Symbol(0x002A, "terminal_item"), 
			new Symbol(0x002B, "rule_sym_action"), 
			new Symbol(0x002C, "rule_sym_virtual"), 
			new Symbol(0x002D, "rule_sym_ref_params"), 
			new Symbol(0x002E, "rule_sym_ref_template"), 
			new Symbol(0x002F, "rule_sym_ref_simple"), 
			new Symbol(0x0030, "rule_def_atom"), 
			new Symbol(0x0031, "rule_def_context"), 
			new Symbol(0x0032, "rule_def_element"), 
			new Symbol(0x0033, "rule_def_tree_action"), 
			new Symbol(0x0034, "rule_def_repetition"), 
			new Symbol(0x0035, "rule_def_fragment"), 
			new Symbol(0x0036, "rule_def_choice"), 
			new Symbol(0x0037, "rule_definition"), 
			new Symbol(0x0038, "rule_template_params"), 
			new Symbol(0x0039, "cf_rule_template"), 
			new Symbol(0x003A, "cf_rule_simple"), 
			new Symbol(0x003B, "cf_rule"), 
			new Symbol(0x003C, "grammar_options"), 
			new Symbol(0x003D, "grammar_terminals"), 
			new Symbol(0x003E, "grammar_cf_rules"), 
			new Symbol(0x003F, "grammar_parency"), 
			new Symbol(0x0040, "cf_grammar"), 
			new Symbol(0x0041, "file"), 
			new Symbol(0x004B, "__V75"), 
			new Symbol(0x004C, "__V76"), 
			new Symbol(0x004D, "__V77"), 
			new Symbol(0x0050, "__V80"), 
			new Symbol(0x0053, "__V83"), 
			new Symbol(0x0056, "__V86"), 
			new Symbol(0x0058, "__V88"), 
			new Symbol(0x0059, "__V89"), 
			new Symbol(0x005A, "__V90"), 
			new Symbol(0x005B, "__V91"), 
			new Symbol(0x005C, "__V92"), 
			new Symbol(0x005E, "__V94"), 
			new Symbol(0x0060, "__V96"), 
			new Symbol(0x0061, "__VAxiom") };
		/// <summary>
		/// The collection of virtuals matched by this parser
		/// </summary>
		/// <remarks>
		/// The virtuals are in an order consistent with the automaton,
		/// so that virtual indices in the automaton can be used to retrieve the virtuals in this table
		/// </remarks>
		private static readonly Symbol[] virtuals = {
			new Symbol(0x0046, "range"), 
			new Symbol(0x004A, "concat"), 
			new Symbol(0x0057, "emptypart") };
		/// <summary>
		/// Initializes a new instance of the parser
		/// </summary>
		/// <param name="lexer">The input lexer</param>
		public HimeGrammarParser(HimeGrammarLexer lexer) : base (commonAutomaton, variables, virtuals, null, lexer) { }
	}
}
