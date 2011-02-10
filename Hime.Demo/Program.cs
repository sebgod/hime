﻿namespace LangTest
{
    public class Program
    {
        static void Compile()
        {
            Hime.Parsers.GrammarBuildOptions Options = new Hime.Parsers.GrammarBuildOptions("Analyzer", new Hime.Parsers.CF.LR.MethodLALR1(), "TestAnalyze.cs");
            Hime.Kernel.Reporting.Reporter Reporter = Options.Reporter;

            Hime.Kernel.Namespace root = Hime.Kernel.Namespace.CreateRoot();
            Hime.Kernel.Resources.ResourceCompiler compiler = new Hime.Kernel.Resources.ResourceCompiler();
            compiler.AddInputFile("Languages\\Test.gram");
            compiler.Compile(root, Reporter);
            Hime.Parsers.Grammar grammar = (Hime.Parsers.Grammar)root.ResolveName(Hime.Kernel.QualifiedName.ParseName("Test"));
            grammar.Build(Options);
            Options.Close();

            Reporter.ExportHTML("LogTest.html", "Grammar Log");
            System.Diagnostics.Process.Start("LogTest.html");
        }

        static void Parse()
        {
            Analyzer.Test_Lexer Lex = new Analyzer.Test_Lexer("(15)");
            Analyzer.Test_Parser Parser = new Analyzer.Test_Parser(Lex);

            Hime.Redist.Parsers.SyntaxTreeNode Root = Parser.Analyse();
            if (Root != null)
            {
                Root = Root.ApplyActions();
                LangTest.WinTreeView Win = new LangTest.WinTreeView(Root);
                Win.ShowDialog();
            }
        }

        static void Main(string[] args)
        {
            //Hime.Kernel.KernelDaemon.GenerateNextStep("D:\\Data\\VisualStudioProjects");
            //Compile();
            Parse();
        }






        static void BuildUnicode()
        {
            string[] classes = { "Lu", "Ll", "Lt", "Lm", "Lo", "L", "Mn", "Mc", "Me", "M", "Nd", "Nl", "No", "N", "Pc", "Pd", "Ps", "Pe", "Pi", "Pf", "Po", "P", "Sm", "Sc", "Sk", "So", "S", "Zs", "Zl", "Zp", "Z", "Cc", "Cf", "Cs", "Co", "Cn", "C" };
            System.Collections.Generic.List<string> code = new System.Collections.Generic.List<string>();
            for (int i = 0; i != classes.Length; i++)
                BuildClass(classes[i], code);
            System.IO.File.WriteAllLines("classes.txt", code.ToArray());
        }
        static void BuildClass(string name, System.Collections.Generic.List<string> code)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            string content = null;
            try { content = client.DownloadString("http://www.fileformat.info/info/unicode/category/" + name + "/list.htm"); }
            catch (System.Exception ex) {
                System.Console.WriteLine("Cannot build class: " + name);
                return;
            }
            System.Text.RegularExpressions.Regex exp = new System.Text.RegularExpressions.Regex("U\\+[0-9A-Za-z]+");
            System.Text.RegularExpressions.MatchCollection matches = exp.Matches(content);
            System.Collections.Generic.List<System.UInt16> results = new System.Collections.Generic.List<System.UInt16>();
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                string temp = match.Value.Substring(2);
                if (temp.Length > 4)
                    continue;
                results.Add(System.Convert.ToUInt16(temp, 16));
            }
            results.Sort();

            if (results.Count == 0)
                return;

            code.Add("UnicodeClass c_" + name + " = new UnicodeClass(\"" + name + "\");");
            System.UInt16 begin = results[0];
            System.UInt16 end = begin;
            for (int i = 1; i != results.Count; i++)
            {
                System.UInt16 current = results[i];
                if (current == end + 1)
                {
                    end = current;
                    continue;
                }
                string range = "c_" + name + ".p_Spans.Add(new UnicodeSpan(0x" + begin.ToString("X") + ", 0x" + end.ToString("X") + "));";
                code.Add(range);
                begin = current;
                end = current;
            }
            string r = "c_" + name + ".p_Spans.Add(new UnicodeSpan(0x" + begin.ToString("X") + ", 0x" + end.ToString("X") + "));";
            code.Add(r);
            code.Add("BuildClasses_Class(c_" + name + ");");
        }
    }
}
