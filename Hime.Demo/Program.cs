﻿namespace LangTest
{
    public class Program
    {        
        static void Parse()
        {
            Analyzer.MathExp_Lexer Lex = new Analyzer.MathExp_Lexer("5 + 6");
            Analyzer.MathExp_Parser Parser = new Analyzer.MathExp_Parser(new Interpreter(), Lex);
            Hime.Redist.Parsers.SyntaxTreeNode Root = Parser.Analyse();

            foreach (Hime.Redist.Parsers.LexerError LexerError in Lex.Errors) System.Console.WriteLine(LexerError.ToString());
            foreach (Hime.Redist.Parsers.ParserError ParserError in Parser.Errors) System.Console.WriteLine(ParserError.ToString());
            if (Root != null)
            {
                Root = Root.ApplyActions();
                LangTest.WinTreeView Win = new LangTest.WinTreeView(Root);
                Win.ShowDialog();
            }
        }

        static void Main(string[] args)
        {
            //Hime.Parsers.CompilationTask Task = new Hime.Parsers.CompilationTask(new string[] { "Languages\\Test.gram" }, "Test", Hime.Parsers.ParsingMethod.LALR1, "Analyser", "Test.cs", true, true);
            //Task.Execute();
            Parse();
        }

        class Interpreter : Analyzer.MathExp_Parser.Actions
        {
            private System.Collections.Generic.Stack<float> p_Stack;

            public float Value { get { return p_Stack.Peek(); } }

            public Interpreter() { p_Stack = new System.Collections.Generic.Stack<float>(); }

            public void OnNumber(Hime.Redist.Parsers.SyntaxTreeNode SubRoot)
            {
                Hime.Redist.Parsers.SyntaxTreeNode node = SubRoot.Children[0];
                Hime.Redist.Parsers.SymbolTokenText token = (Hime.Redist.Parsers.SymbolTokenText)node.Symbol;
                float value = System.Convert.ToSingle(token.ValueText);
                p_Stack.Push(value);
            }

            public void OnMult(Hime.Redist.Parsers.SyntaxTreeNode SubRoot)
            {
                float right = p_Stack.Pop();
                float left = p_Stack.Pop();
                p_Stack.Push(left * right);
            }

            public void OnDiv(Hime.Redist.Parsers.SyntaxTreeNode SubRoot)
            {
                float right = p_Stack.Pop();
                float left = p_Stack.Pop();
                p_Stack.Push(left / right);
            }

            public void OnPlus(Hime.Redist.Parsers.SyntaxTreeNode SubRoot)
            {
                float right = p_Stack.Pop();
                float left = p_Stack.Pop();
                p_Stack.Push(left + right);
            }

            public void OnMinus(Hime.Redist.Parsers.SyntaxTreeNode SubRoot)
            {
                float right = p_Stack.Pop();
                float left = p_Stack.Pop();
                p_Stack.Push(left - right);
            }
        }
    }
}
