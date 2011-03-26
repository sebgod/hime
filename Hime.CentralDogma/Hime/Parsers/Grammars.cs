﻿using System.Collections.Generic;

namespace Hime.Parsers
{
    public abstract class Grammar : Hime.Kernel.Symbol
    {
        protected Hime.Kernel.Symbol p_Parent;
        protected Hime.Kernel.QualifiedName p_CompleteName;

        public override Hime.Kernel.Symbol Parent { get { return p_Parent; } }
        public override Hime.Kernel.QualifiedName CompleteName { get { return p_CompleteName; } }

        protected override void SymbolSetParent(Hime.Kernel.Symbol Symbol){ p_Parent = Symbol; }
        protected override void SymbolSetCompleteName(Hime.Kernel.QualifiedName Name) { p_CompleteName = Name; }

        public abstract bool Build(GrammarBuildOptions Options);
    }


    public sealed class GrammarBuildOptions
    {
        private string p_Namespace;
        private Hime.Kernel.Reporting.Reporter p_Log;
        private bool p_Drawvisual;
        private ParserGenerator p_Method;
        private System.IO.StreamWriter p_LexerWriter;
        private System.IO.StreamWriter p_ParserWriter;
        private string p_DocumentationDir;

        public string Namespace { get { return p_Namespace; } }
        public Hime.Kernel.Reporting.Reporter Reporter { get { return p_Log; } }
        public bool DrawVisual { get { return p_Drawvisual; } }
        public ParserGenerator ParserGenerator { get { return p_Method; } }
        public System.IO.StreamWriter LexerWriter { get { return p_LexerWriter; } }
        public System.IO.StreamWriter ParserWriter { get { return p_ParserWriter; } }
        public string DocumentationDir { get { return p_DocumentationDir; } }

        public GrammarBuildOptions(Hime.Kernel.Reporting.Reporter Reporter, string Namespace, ParserGenerator Generator, string File, string DocDir)
        {
            p_Namespace = Namespace;
            p_Log = Reporter;
            p_Drawvisual = false;
            p_Method = Generator;
            p_LexerWriter = new System.IO.StreamWriter(File, false, System.Text.Encoding.UTF8);
            p_ParserWriter = p_LexerWriter;
            p_LexerWriter.WriteLine("using System.Collections.Generic;");
            p_LexerWriter.WriteLine("");
            p_LexerWriter.WriteLine("namespace " + Namespace);
            p_LexerWriter.WriteLine("{");
            p_DocumentationDir = DocDir;
        }
        public GrammarBuildOptions(Hime.Kernel.Reporting.Reporter Reporter, string Namespace, ParserGenerator Generator, string FileLexer, string FileParser, string DocDir)
        {
            p_Namespace = Namespace;
            p_Log = Reporter;
            p_Drawvisual = false;
            p_Method = Generator;
            p_LexerWriter = new System.IO.StreamWriter(FileLexer, false, System.Text.Encoding.UTF8);
            p_LexerWriter.WriteLine("using System.Collections.Generic;");
            p_LexerWriter.WriteLine("");
            p_LexerWriter.WriteLine("namespace " + Namespace);
            p_LexerWriter.WriteLine("{");
            p_ParserWriter = new System.IO.StreamWriter(FileParser, false, System.Text.Encoding.UTF8);
            p_ParserWriter.WriteLine("using System.Collections.Generic;");
            p_ParserWriter.WriteLine("");
            p_ParserWriter.WriteLine("namespace " + Namespace);
            p_ParserWriter.WriteLine("{");
            p_DocumentationDir = DocDir;
        }

        public void Close()
        {
            p_LexerWriter.WriteLine("}");
            p_LexerWriter.Close();
            if (p_ParserWriter != p_LexerWriter)
            {
                p_ParserWriter.WriteLine("}");
                p_ParserWriter.Close();
            }
        }
    }

    public interface ParserData
    {
        ParserGenerator Generator { get; }
        bool Export(GrammarBuildOptions Options);
        System.Xml.XmlNode SerializeXML(System.Xml.XmlDocument Document);
        void SerializeVisual(Kernel.Graphs.DOTSerializer Serializer);
    }

    public interface ParserGenerator
    {
        string Name { get; }
        ParserData Build(Grammar Grammar, Hime.Kernel.Reporting.Reporter Reporter);
    }
}
