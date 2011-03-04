﻿using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace Hime.HimeCC
{
    class Options
    {
        public Options()
        {
            Inputs = new string[] { };
            GrammarName = "none";
            Namespace = "";
            Method = Parsers.ParsingMethod.LALR1;
            LexerFile = null;
            ParserFile = "none.cs";
            ExportHTMLLog = true;
        }

        [OptionArray("i", "input", Required=true, HelpText="Input grammar files")]
        public string[] Inputs;

        [Option("g", "grammar", Required=true, HelpText="Name of the grammar for which a parser shall be generated")]
        public string GrammarName;

        [Option("n", "namespace", Required = true, HelpText = "Namespace for the generated Lexer and Parser classes")]
        public string Namespace;

        [Option("m", "method", Required = true, HelpText = "Name of the parsing method to use: LR0|LR1|LALR1|RNGLR1|RNGLALR1")]
        public Parsers.ParsingMethod Method;

        [Option(null, "lexer", Required = false, HelpText = "Path and name of the file for the generated lexer")]
        public string LexerFile;

        [Option(null, "parser", Required = true, HelpText = "Path and name of the file for the generated parser")]
        public string ParserFile;

        [Option("l", "log", Required = false, HelpText = "True to export the generation log (HTML file)")]
        public bool ExportHTMLLog;

        [HelpOption("h", "help", HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            CommandLine.Text.HelpText help = new CommandLine.Text.HelpText(assembly.FullName);
            help.AdditionalNewLineAfterOption = true;
            help.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of");
            help.AddPreOptionsLine("the LGPL License <http://www.gnu.org/licenses/lgpl.html>.");
            help.AddPreOptionsLine("Usage: himecc -i MyGram.gram -g MyGrammar -n Analyser -m LALR1 --parser MyGram.cs");
            help.AddOptions(this);
            return help;
        }
    }
}
