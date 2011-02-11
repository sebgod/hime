﻿namespace LangTest
{
    public class Program
    {
        static void Compile()
        {
            Hime.Kernel.Reporting.Reporter Reporter = new Hime.Kernel.Reporting.Reporter(typeof(Program));
            Hime.Kernel.Namespace root = Hime.Kernel.Namespace.CreateRoot();
            Hime.Kernel.Resources.ResourceCompiler compiler = new Hime.Kernel.Resources.ResourceCompiler();
            compiler.AddInputFile("Languages\\MathExp.gram");
            compiler.Compile(root, Reporter);
            Hime.Parsers.Grammar grammar = (Hime.Parsers.Grammar)root.ResolveName(Hime.Kernel.QualifiedName.ParseName("MathExp"));
            Hime.Parsers.GrammarBuildOptions Options = new Hime.Parsers.GrammarBuildOptions(Reporter, "Analyzer", new Hime.Parsers.CF.LR.MethodLALR1(), "MathExp.cs");
            grammar.Build(Options);
            Options.Close();
            Reporter.ExportHTML("MathExp.html", "Grammar Log");
            System.Diagnostics.Process.Start("MathExp.html");
        }

        static void Parse()
        {
            Interpreter interpreter = new Interpreter();
            Analyzer.MathExp_Lexer Lex = new Analyzer.MathExp_Lexer("(2 + 3) * (5 - 2)");
            Analyzer.MathExp_Parser Parser = new Analyzer.MathExp_Parser(interpreter, Lex);

            Hime.Redist.Parsers.SyntaxTreeNode Root = Parser.Analyse();
            System.Console.WriteLine("result = " + interpreter.Value);
            if (Root != null)
            {
                Root = Root.ApplyActions();
                LangTest.WinTreeView Win = new LangTest.WinTreeView(Root);
                Win.ShowDialog();
            }
        }

        static void Main(string[] args)
        {
            Hime.Kernel.KernelDaemon.GenerateNextStep("D:\\Data\\VisualStudioProjects");
            //Compile();
            //Parse();
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
