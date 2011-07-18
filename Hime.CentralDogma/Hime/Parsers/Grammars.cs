﻿using System.Collections.Generic;

namespace Hime.Parsers
{
    public abstract class Grammar : Hime.Kernel.Symbol
    {
        protected Hime.Kernel.QualifiedName completeName;

        public override Hime.Kernel.QualifiedName CompleteName { get { return completeName; } }

        protected override void SymbolSetParent(Hime.Kernel.Symbol symbol){ this.Parent = symbol; }
        protected override void SymbolSetCompleteName(Hime.Kernel.QualifiedName Name) { completeName = Name; }

        public abstract bool Build(CompilationTask options);
    }

    public interface ParserData
    {
        ParserGenerator Generator { get; }
        bool Export(IList<Terminal> expected, CompilationTask options);
        System.Xml.XmlNode SerializeXML(System.Xml.XmlDocument Document);
        List<string> SerializeVisuals(string directory, CompilationTask options);
    }

    public interface ParserGenerator
    {
        string Name { get; }
        ParserData Build(Grammar Grammar, Hime.Kernel.Reporting.Reporter Reporter);
    }
}
