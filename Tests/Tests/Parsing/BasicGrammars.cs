/**********************************************************************
* Copyright (c) 2014 Laurent Wouters and others
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU Lesser General Public License as
* published by the Free Software Foundation, either version 3
* of the License, or (at your option) any later version.
* 
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU Lesser General Public License for more details.
* 
* You should have received a copy of the GNU Lesser General
* Public License along with this program.
* If not, see <http://www.gnu.org/licenses/>.
* 
* Contributors:
*     Laurent Wouters - lwouters@xowl.org
**********************************************************************/

using Hime.CentralDogma;
using NUnit.Framework;

namespace Hime.Tests
{
	/// <summary>
	/// Tests the basic grammar features
	/// </summary>
	[TestFixture]
	public class ParsingBasicGrammars : BaseParseSuite
	{
		/// <summary>
		/// Tests that the correct grammar variable is selected as the axiom of the grammar
		/// </summary>
		[Test]
        public void Test_Option_Axiom_CorrectVariable()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} rules { b->; e->; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "", "e");
        }
		
        /// <summary>
        /// Tests the Separator option with spaces
        /// </summary>
		[Test]
        public void Test_Option_Separator_Space()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\"; Separator=\"SEPARATOR\";} terminals {SEPARATOR->' '+; A->'a'; } rules { e->A+; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a a   a     a", "e(A A A A)");
        }
		
        /// <summary>
        /// Tests the Separator option with an arbitrary character
        /// </summary>
        [Test]
        public void Test_Option_Separator_ArbitraryCharacter()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\"; Separator=\"SEPARATOR\";} terminals {SEPARATOR->'x'+; A->'a'; } rules { e->A+; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "axaxxxaxxxxa", "e(A A A A)");
        }
		
		/// <summary>
		/// Tests parsing of a single-character terminal
		/// </summary>
		[Test]
        public void Test_Terminals_SingleCharacter()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a';} rules { e->A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a", "e(A='a')");
        }
		
        /// <summary>
		/// Tests parsing of terminals with the escape sequence for backslash
		/// </summary>
		[Test]
        public void Test_Terminals_EscapeSequenceBackslash()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; X->'\\\\';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\\a", "e(A X='\\' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with the escape sequence for single quote
		/// </summary>
		[Test]
        public void Test_Terminals_EscapeSequenceSingleQuote()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; X->'\\'';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a'a", "e(A X A)");
        }
		
        /// <summary>
		/// Tests parsing of terminals with the escape sequence for Unicode character 0
		/// </summary>
		[Test]
        public void Test_Terminals_EscapeSequenceUnicode0()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; X->'\\0';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\u0000a", "e(A X='\u0000' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with the escape sequence for Unicode character 7 (Alert)
		/// </summary>
		[Test]
        public void Test_Terminals_EscapeSequenceUnicode7()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; X->'\\a';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\u0007a", "e(A X='\u0007' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with the escape sequence for Unicode character 8 (Backspace)
		/// </summary>
		[Test]
        public void Test_Terminals_EscapeSequenceUnicode8()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; X->'\\b';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\u0008a", "e(A X='\u0008' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with the escape sequence for Unicode character 9 (Horizontal tab)
		/// </summary>
		[Test]
        public void Test_Terminals_EscapeSequenceUnicode9()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; X->'\\t';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\ta", "e(A X='\t' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with the escape sequence for Unicode character 10 (New Line)
		/// </summary>
		[Test]
        public void Test_Terminals_EscapeSequenceUnicode10()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; X->'\\n';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\na", "e(A X='\n' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with the escape sequence for Unicode character 11 (Vertical tab)
		/// </summary>
		[Test]
        public void Test_Terminals_EscapeSequenceUnicode11()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; X->'\\v';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\va", "e(A X='\v' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with the escape sequence for Unicode character 12 (form feed)
		/// </summary>
		[Test]
        public void Test_Terminals_EscapeSequenceUnicode12()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; X->'\\f';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\fa", "e(A X='\f' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with the escape sequence for Unicode character 13 (Carriage return)
		/// </summary>
		[Test]
        public void Test_Terminals_EscapeSequenceUnicode13()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; X->'\\r';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\ra", "e(A X='\r' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with more than one character
		/// </summary>
		[Test]
        public void Test_Terminals_MultipleCharacters()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'abc';} rules { e->A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "abc", "e(A='abc')");
        }
        
        /// <summary>
		/// Tests parsing of terminals with unicode code point
		/// </summary>
		[Test]
        public void Test_Terminals_UnicodeCodepoint_1234()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; X->0x1234;} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\u1234a", "e(A X='\u1234' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with unicode code point
		/// </summary>
		[Test]
        public void Test_Terminals_UnicodeCodepoint_Range()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; X->0x1234 .. 0x5678;} rules { e->X X X; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "\u1234\u3333\u5678", "e(X='\u1234' X='\u3333' X='\u5678')");
        }
        
		/// <summary>
		/// Tests parsing of terminals with single wildcard
		/// </summary>
		[Test]
        public void Test_Terminals_Wildcard_Single()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->.; A->'a';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "aba", "e(A X='b' A)");
        }
		
        /// <summary>
		/// Tests parsing of terminals with single wildcard
		/// </summary>
		[Test]
        public void Test_Terminals_Wildcard_SpecialCharacter_0()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->.; A->'a';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\u0000a", "e(A X='\u0000' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with single wildcard
		/// </summary>
		[Test]
        public void Test_Terminals_Wildcard_SpecialCharacter_1234()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->.; A->'a';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\u1234a", "e(A X='\u1234' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with single wildcard
		/// </summary>
		[Test]
        public void Test_Terminals_Wildcard_SpecialCharacter_5678()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->.; A->'a';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\u5678a", "e(A X='\u5678' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with single wildcard
		/// </summary>
		[Test]
        public void Test_Terminals_Wildcard_NewLine()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->.; A->'a';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\na", "e(A X='\n' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with single wildcard
		/// </summary>
		[Test]
        public void Test_Terminals_Wildcard_CarriageReturn()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->.; A->'a';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a\ra", "e(A X='\r' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with simple character class
		/// </summary>
		[Test]
        public void Test_Terminals_Class_SimpleCharacter()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->[xyz]; A->'a';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "aya", "e(A X='y' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with ranged charater class
		/// </summary>
		[Test]
        public void Test_Terminals_Class_Range()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->[x-z]; A->'a';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "aya", "e(A X='y' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with ranged charater class
		/// </summary>
		[Test]
        public void Test_Terminals_Class_RangeLeftBorder()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->[x-z]; A->'a';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "axa", "e(A X='x' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with ranged charater class
		/// </summary>
		[Test]
        public void Test_Terminals_Class_RangeRightBorder()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->[x-z]; A->'a';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "aza", "e(A X='z' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with negative ranged charater class
		/// </summary>
		[Test]
        public void Test_Terminals_Class_NegativeRange_Excluded()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->[^x-z]; A->'a';} rules { e->A X A; } }";
			ParsingFails(grammar, "Test", ParsingMethod.LALR1, "aya");
        }
        
        /// <summary>
		/// Tests parsing of terminals with negative ranged charater class
		/// </summary>
		[Test]
        public void Test_Terminals_Class_NegativeRange_ExcludedLeftBorder()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->[^x-z]; A->'a';} rules { e->A X A; } }";
			ParsingFails(grammar, "Test", ParsingMethod.LALR1, "axa");
        }
        
        /// <summary>
		/// Tests parsing of terminals with negative ranged charater class
		/// </summary>
		[Test]
        public void Test_Terminals_Class_NegativeRange_ExcludedRightBorder()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->[^x-z]; A->'a';} rules { e->A X A; } }";
			ParsingFails(grammar, "Test", ParsingMethod.LALR1, "aza");
        }
        
        /// <summary>
		/// Tests parsing of terminals with negative ranged charater class
		/// </summary>
		[Test]
        public void Test_Terminals_Class_NegativeRange_Included()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->[^x-z]; A->'a';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "aba", "e(A X='b' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with ranged charater class (multiple ranges)
		/// </summary>
		[Test]
        public void Test_Terminals_Class_MultiRange_First()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->[x-z0-9]; A->'a';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "aya", "e(A X='y' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with ranged charater class (multiple ranges)
		/// </summary>
		[Test]
        public void Test_Terminals_Class_MultiRange_Second()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->[x-z0-9]; A->'a';} rules { e->A X A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a5a", "e(A X='5' A)");
        }
        
        /// <summary>
		/// Tests parsing of terminals with negative ranged charater class (multiple ranges)
		/// </summary>
		[Test]
        public void Test_Terminals_Class_NegativeMultiRange_First()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->[^x-z0-9]; A->'a';} rules { e->A X A; } }";
			ParsingFails(grammar, "Test", ParsingMethod.LALR1, "aya");
        }
        
        /// <summary>
		/// Tests parsing of terminals with negative ranged charater class (multiple ranges)
		/// </summary>
		[Test]
        public void Test_Terminals_Class_NegativeMultiRange_Second()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {X->[^x-z0-9]; A->'a';} rules { e->A X A; } }";
			ParsingFails(grammar, "Test", ParsingMethod.LALR1, "a5a");
        }
        
        
        
        
		/// <summary>
		/// Tests parsing of empty input
		/// </summary>
		[Test]
        public void Test_Minimal_Empty()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"a\";} rules { a->; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "", "a");
        }

		/// <summary>
		/// Tests parsing of minimal non-empty input
		/// </summary>
		[Test]
        public void Test_Minimal_Single_MatchOne()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a';} rules { e->A; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a", "e(A='a')");
        }

		/// <summary>
		/// Tests parsing fails on empty input when single element was expected
		/// </summary>
		[Test]
        public void Test_Minimal_Single_FailsEmpty()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a';} rules { e->A; } }";
			ParsingFails(grammar, "Test", ParsingMethod.LALR1, "");
        }

		/// <summary>
		/// Tests parsing fails on wrong input when single element was expected
		/// </summary>
		[Test]
        public void Test_Minimal_Single_FailsWrongInput()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a';} rules { e->A; } }";
			ParsingFails(grammar, "Test", ParsingMethod.LALR1, "b");
        }

		/// <summary>
		/// Tests parsing with the optional operator on empty input
		/// </summary>
		[Test]
        public void Test_Grammar_OptionalOperator_MatchZero()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a';} rules { e->A?; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "", "e");
        }

		[Test]
        public void Test_Grammar_OptionalOperator_MatchOne()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a';} rules { e->A?; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a", "e(A)");
        }

		[Test]
        public void Test_Grammar_StarOperator_MatchZero()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a';} rules { e->A*; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "", "e");
        }

		[Test]
        public void Test_Grammar_StarOperator_MatchOne()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a';} rules { e->A*; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a", "e(A)");
        }

		[Test]
        public void Test_Grammar_StarOperator_MatchMoreThanOne()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a';} rules { e->A*; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "aaa", "e(A A A)");
        }

		[Test]
        public void Test_Grammar_PlusOperator_FailsOnZeo()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a';} rules { e->A+; } }";
			ParsingFails(grammar, "Test", ParsingMethod.LALR1, "");
        }

		[Test]
        public void Test_Grammar_PlusOperator_MatchOne()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a';} rules { e->A+; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a", "e(A)");
        }

		[Test]
        public void Test_Grammar_PlusOperator_MatchMoreThanOne()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a';} rules { e->A+; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "aaa", "e(A A A)");
        }

		[Test]
        public void Test_Grammar_UnionOperator_Left()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; B->'b';} rules { e->A|B; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a", "e(A)");
        }

		[Test]
        public void Test_Grammar_UnionOperator_Right()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; B->'b';} rules { e->A|B; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "b", "e(B)");
        }




		[Test]
        public void Test_Grammar_OperatorComposition0()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; B->'b';} rules { e->(A|B)*; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "", "e");
        }

		[Test]
        public void Test_Grammar_OperatorComposition1()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; B->'b';} rules { e->(A|B)*; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "a", "e(A)");
        }

		[Test]
        public void Test_Grammar_OperatorComposition2()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; B->'b';} rules { e->(A|B)*; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "b", "e(B)");
        }

		[Test]
        public void Test_Grammar_OperatorComposition3()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; B->'b';} rules { e->(A|B)*; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "aba", "e(A B A)");
        }

		[Test]
        public void Test_Grammar_OperatorComposition4()
        {
			SetTestDirectory();
			string grammar = "cf grammar Test { options {Axiom=\"e\";} terminals {A->'a'; B->'b';} rules { e->(A|B)*; } }";
			ParsingMatches(grammar, "Test", ParsingMethod.LALR1, "bab", "e(B A B)");
        }

	}
}

