/*
 * WARNING: this file has been generated by
 * Hime Parser Generator 2.0.0.0
 */
using System.Collections.Generic;
using Hime.Redist;
using Hime.Redist.Parsers;

namespace Hime.SDK.Input
{
	/// <summary>
	/// Represents a parser
	/// </summary>
	internal class CommandLineParser : LRkParser
	{
		/// <summary>
		/// The automaton for this parser
		/// </summary>
		private static readonly LRkAutomaton commonAutomaton = LRkAutomaton.Find(typeof(CommandLineParser), "CommandLineParser.bin");
		/// <summary>
		/// Contains the constant IDs for the variables and virtuals in this parser
		/// </summary>
		public class ID
		{
			/// <summary>
			/// The unique identifier for variable value
			/// </summary>
			public const int value = 0x0009;
			/// <summary>
			/// The unique identifier for variable argument
			/// </summary>
			public const int argument = 0x000A;
			/// <summary>
			/// The unique identifier for variable values
			/// </summary>
			public const int values = 0x000B;
			/// <summary>
			/// The unique identifier for variable arguments
			/// </summary>
			public const int arguments = 0x000C;
			/// <summary>
			/// The unique identifier for variable line
			/// </summary>
			public const int line = 0x000D;
			/// <summary>
			/// The unique identifier for variable _Axiom_
			/// </summary>
			public const int _Axiom_ = 0x0011;
		}
		/// <summary>
		/// The collection of variables matched by this parser
		/// </summary>
		/// <remarks>
		/// The variables are in an order consistent with the automaton,
		/// so that variable indices in the automaton can be used to retrieve the variables in this table
		/// </remarks>
		private static readonly Symbol[] variables = {
			new Symbol(0x0009, "value"), 
			new Symbol(0x000A, "argument"), 
			new Symbol(0x000B, "values"), 
			new Symbol(0x000C, "arguments"), 
			new Symbol(0x000D, "line"), 
			new Symbol(0x000E, "_gen_V14"), 
			new Symbol(0x000F, "_gen_V15"), 
			new Symbol(0x0010, "_gen_V16"), 
			new Symbol(0x0011, "_Axiom_") };
		/// <summary>
		/// The collection of virtuals matched by this parser
		/// </summary>
		/// <remarks>
		/// The virtuals are in an order consistent with the automaton,
		/// so that virtual indices in the automaton can be used to retrieve the virtuals in this table
		/// </remarks>
		private static readonly Symbol[] virtuals = {
 };
		/// <summary>
		/// Initializes a new instance of the parser
		/// </summary>
		/// <param name="lexer">The input lexer</param>
		public CommandLineParser(CommandLineLexer lexer) : base (commonAutomaton, variables, virtuals, null, lexer) { }
	}
}
