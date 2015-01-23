/*
 * WARNING: this file has been generated by
 * Hime Parser Generator 2.0.0.0
 */
using System.Collections.Generic;
using System.IO;
using Hime.Redist;
using Hime.Redist.Lexer;

namespace Hime.SDK.Input
{
	/// <summary>
	/// Represents a lexer
	/// </summary>
	internal class HimeGrammarLexer : ContextFreeLexer
	{
		/// <summary>
		/// The automaton for this lexer
		/// </summary>
		private static readonly Automaton commonAutomaton = Automaton.Find(typeof(HimeGrammarLexer), "HimeGrammarLexer.bin");
		/// <summary>
		/// Contains the constant IDs for the terminals for this lexer
		/// </summary>
		public class ID
		{
			/// <summary>
			/// The unique identifier for terminal NAME
			/// </summary>
			public const int NAME = 0x0009;
			/// <summary>
			/// The unique identifier for terminal LITERAL_ANY
			/// </summary>
			public const int LITERAL_ANY = 0x000D;
			/// <summary>
			/// The unique identifier for terminal OPERATOR_OPTIONAL
			/// </summary>
			public const int OPERATOR_OPTIONAL = 0x0014;
			/// <summary>
			/// The unique identifier for terminal OPERATOR_ZEROMORE
			/// </summary>
			public const int OPERATOR_ZEROMORE = 0x0015;
			/// <summary>
			/// The unique identifier for terminal OPERATOR_ONEMORE
			/// </summary>
			public const int OPERATOR_ONEMORE = 0x0016;
			/// <summary>
			/// The unique identifier for terminal OPERATOR_UNION
			/// </summary>
			public const int OPERATOR_UNION = 0x0017;
			/// <summary>
			/// The unique identifier for terminal OPERATOR_DIFFERENCE
			/// </summary>
			public const int OPERATOR_DIFFERENCE = 0x0018;
			/// <summary>
			/// The unique identifier for terminal TREE_ACTION_PROMOTE
			/// </summary>
			public const int TREE_ACTION_PROMOTE = 0x0019;
			/// <summary>
			/// The unique identifier for terminal TREE_ACTION_DROP
			/// </summary>
			public const int TREE_ACTION_DROP = 0x001A;
			/// <summary>
			/// The unique identifier for terminal SEPARATOR
			/// </summary>
			public const int SEPARATOR = 0x0007;
			/// <summary>
			/// The unique identifier for terminal INTEGER
			/// </summary>
			public const int INTEGER = 0x000A;
			/// <summary>
			/// The unique identifier for terminal LITERAL_STRING
			/// </summary>
			public const int LITERAL_STRING = 0x000C;
			/// <summary>
			/// The unique identifier for terminal UNICODE_SPAN_MARKER
			/// </summary>
			public const int UNICODE_SPAN_MARKER = 0x0013;
			/// <summary>
			/// The unique identifier for terminal ESCAPEES
			/// </summary>
			public const int ESCAPEES = 0x000B;
			/// <summary>
			/// The unique identifier for terminal UNICODE_CODEPOINT
			/// </summary>
			public const int UNICODE_CODEPOINT = 0x0012;
			/// <summary>
			/// The unique identifier for terminal LITERAL_CLASS
			/// </summary>
			public const int LITERAL_CLASS = 0x000F;
			/// <summary>
			/// The unique identifier for terminal LITERAL_TEXT
			/// </summary>
			public const int LITERAL_TEXT = 0x000E;
			/// <summary>
			/// The unique identifier for terminal UNICODE_BLOCK
			/// </summary>
			public const int UNICODE_BLOCK = 0x0010;
			/// <summary>
			/// The unique identifier for terminal UNICODE_CATEGORY
			/// </summary>
			public const int UNICODE_CATEGORY = 0x0011;
			/// <summary>
			/// The unique identifier for terminal BLOCK_RULES
			/// </summary>
			public const int BLOCK_RULES = 0x001D;
			/// <summary>
			/// The unique identifier for terminal BLOCK_OPTIONS
			/// </summary>
			public const int BLOCK_OPTIONS = 0x001B;
			/// <summary>
			/// The unique identifier for terminal BLOCK_CONTEXT
			/// </summary>
			public const int BLOCK_CONTEXT = 0x001E;
			/// <summary>
			/// The unique identifier for terminal BLOCK_TERMINALS
			/// </summary>
			public const int BLOCK_TERMINALS = 0x001C;
		}
		/// <summary>
		/// Contains the constant IDs for the contexts for this lexer
		/// </summary>
		public class Context
		{
			/// <summary>
			/// The unique identifier for the default context
			/// </summary>
			public const int DEFAULT = 0;
		}
		/// <summary>
		/// The collection of terminals matched by this lexer
		/// </summary>
		/// <remarks>
		/// The terminals are in an order consistent with the automaton,
		/// so that terminal indices in the automaton can be used to retrieve the terminals in this table
		/// </remarks>
		private static readonly Symbol[] terminals = {
			new Symbol(0x0001, "ε"),
			new Symbol(0x0002, "$"),
			new Symbol(0x0009, "NAME"),
			new Symbol(0x000D, "LITERAL_ANY"),
			new Symbol(0x0014, "OPERATOR_OPTIONAL"),
			new Symbol(0x0015, "OPERATOR_ZEROMORE"),
			new Symbol(0x0016, "OPERATOR_ONEMORE"),
			new Symbol(0x0017, "OPERATOR_UNION"),
			new Symbol(0x0018, "OPERATOR_DIFFERENCE"),
			new Symbol(0x0019, "TREE_ACTION_PROMOTE"),
			new Symbol(0x001A, "TREE_ACTION_DROP"),
			new Symbol(0x0040, "="),
			new Symbol(0x0041, ";"),
			new Symbol(0x0042, "("),
			new Symbol(0x0043, ")"),
			new Symbol(0x0045, "{"),
			new Symbol(0x0046, ","),
			new Symbol(0x0047, "}"),
			new Symbol(0x004E, "<"),
			new Symbol(0x0050, ">"),
			new Symbol(0x0058, ":"),
			new Symbol(0x0007, "SEPARATOR"),
			new Symbol(0x000A, "INTEGER"),
			new Symbol(0x000C, "LITERAL_STRING"),
			new Symbol(0x0013, "UNICODE_SPAN_MARKER"),
			new Symbol(0x004C, "->"),
			new Symbol(0x000B, "ESCAPEES"),
			new Symbol(0x0012, "UNICODE_CODEPOINT"),
			new Symbol(0x000F, "LITERAL_CLASS"),
			new Symbol(0x000E, "LITERAL_TEXT"),
			new Symbol(0x0010, "UNICODE_BLOCK"),
			new Symbol(0x0011, "UNICODE_CATEGORY"),
			new Symbol(0x001D, "BLOCK_RULES"),
			new Symbol(0x001B, "BLOCK_OPTIONS"),
			new Symbol(0x001E, "BLOCK_CONTEXT"),
			new Symbol(0x005A, "grammar"),
			new Symbol(0x001C, "BLOCK_TERMINALS") };
		/// <summary>
		/// Initializes a new instance of the lexer
		/// </summary>
		/// <param name="input">The lexer's input</param>
		public HimeGrammarLexer(string input) : base(commonAutomaton, terminals, 0x0007, input) {}
		/// <summary>
		/// Initializes a new instance of the lexer
		/// </summary>
		/// <param name="input">The lexer's input</param>
		public HimeGrammarLexer(TextReader input) : base(commonAutomaton, terminals, 0x0007, input) {}
	}
}
