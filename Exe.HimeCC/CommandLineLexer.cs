/*
 * WARNING: this file has been generated by
 * Hime Parser Generator 1.0.0.0
 */

using System.Collections.Generic;
using Hime.Redist;
using Hime.Redist.Lexer;

namespace Hime.HimeCC.CL
{
	/// <summary>
	/// Represents a lexer
	/// </summary>
	internal class CommandLineLexer : Lexer
	{
		/// <summary>
		/// The automaton for this lexer
		/// </summary>
		private static readonly Automaton automaton = Automaton.Find(typeof(CommandLineLexer), "CommandLineLexer.bin");
		/// <summary>
		/// Contains the constant IDs for the terminals for this lexer
		/// </summary>
		public sealed class ID
		{
			/// <summary>
			/// The unique identifier for terminal ARG_VALUE_NAME
			/// </summary>
			public const int ARG_VALUE_NAME = 0x7;
			/// <summary>
			/// The unique identifier for terminal ARG_VALUE_NUMBER
			/// </summary>
			public const int ARG_VALUE_NUMBER = 0x8;
			/// <summary>
			/// The unique identifier for terminal WHITE_SPACE
			/// </summary>
			public const int WHITE_SPACE = 0x3;
			/// <summary>
			/// The unique identifier for terminal ARG_NAME
			/// </summary>
			public const int ARG_NAME = 0x4;
			/// <summary>
			/// The unique identifier for terminal ARG_VALUE_QUOTE
			/// </summary>
			public const int ARG_VALUE_QUOTE = 0x5;
		}
		/// <summary>
		/// The collection of terminals matched by this lexer
		/// </summary>
		/// <remarks>
		/// The terminals are in an order consistent with the automaton,
		/// so that terminal indices in the automaton can be used to retrieve the terminals in this table
		/// </remarks>
		private static readonly Symbol[] terminals = {
			new Symbol(0x1, "ε"),
			new Symbol(0x2, "$"),
			new Symbol(0x7, "ARG_VALUE_NAME"),
			new Symbol(0x8, "ARG_VALUE_NUMBER"),
			new Symbol(0x3, "WHITE_SPACE"),
			new Symbol(0x4, "ARG_NAME"),
			new Symbol(0x5, "ARG_VALUE_QUOTE") };
		/// <summary>
		/// Initializes a new instance of the lexer
		/// </summary>
		/// <param name="input">The lexer's input</param>
		public CommandLineLexer(string input) : base(automaton, terminals, 0x3, new System.IO.StringReader(input)) {}
		/// <summary>
		/// Initializes a new instance of the lexer
		/// </summary>
		/// <param name="input">The lexer's input</param>
		public CommandLineLexer(System.IO.TextReader input) : base(automaton, terminals, 0x3, input) {}
	}
}
