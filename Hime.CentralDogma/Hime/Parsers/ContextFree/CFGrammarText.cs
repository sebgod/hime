﻿/*
 * Author: Charles Hymans
 * Date: 18/07/2011
 * Time: 23:10
 * 
 */
using System;
using System.IO;
using System.Collections.Generic;
using Hime.Kernel.Reporting;

namespace Hime.Parsers.ContextFree
{
	public sealed class CFGrammarText : CFGrammar
    {
        public CFGrammarText(string name) : base(name) { }

        public override void Inherit(Grammar parent)
        {
            InheritOptions(parent);
            InheritActions(parent);
            InheritVirtuals(parent);
            foreach (TerminalText terminal in parent.Terminals)
            {
                TerminalText clone = AddTerminalText(terminal.LocalName, terminal.NFA.Clone(false), terminal.SubGrammar);
                clone.NFA.StateExit.Final = clone;
            }
            InheritTemplateRules(parent as CFGrammar);
            InheritVariables(parent as CFGrammar);
        }

        public override Grammar Clone()
        {
            Grammar result = new CFGrammarText(name);
            result.Inherit(this);
            return result;
        }

        private Automata.DFA PrepareDFA(Reporter log)
        {
            log.Info("Grammar", "Generating DFA for Terminals ...");

            // Construct a global NFA for all the terminals
            Automata.NFA final = new Automata.NFA();
            final.StateEntry = final.AddNewState();
            foreach (TerminalText terminal in terminals.Values)
            {
                Automata.NFA sub = terminal.NFA.Clone();
                final.InsertSubNFA(sub);
                final.StateEntry.AddTransition(Automata.NFA.Epsilon, sub.StateEntry);
            }
            // Construct the equivalent DFA and minimize it
            Automata.DFA finalDFA = new Automata.DFA(final);
            finalDFA = finalDFA.Minimize();
            finalDFA.RepackTransitions();

            log.Info("Grammar", "Done !");
            return finalDFA;
        }

        public override LexerData GetLexerData(Reporter reporter)
        {
            Automata.DFA finalDFA = PrepareDFA(reporter);
            Terminal separator = null;
            if (options.ContainsKey("Separator"))
                separator = terminals[options["Separator"]];
            return new TextLexerData(finalDFA, separator);
        }
    }
}
