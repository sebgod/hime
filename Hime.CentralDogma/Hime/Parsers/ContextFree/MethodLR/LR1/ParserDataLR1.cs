﻿using System.Collections.Generic;

namespace Hime.Parsers.CF.LR
{
    class ParserDataLR1 : ParserData
    {
        protected Kernel.Reporting.Reporter reporter;
        protected string terminalsAccessor;

        public ParserDataLR1(ParserGenerator generator, CFGrammar gram, Graph graph) : base(generator, gram, graph) { }


        public override bool Export(IList<Terminal> expected, CompilationTask options)
        {
            reporter = options.Reporter;
            terminals = new List<Terminal>(expected);
            debug = options.ExportDebug;
            terminalsAccessor = this.GrammarName + "_Lexer.terminals";
            stream = options.ParserWriter;
            stream.Write("    " + options.GeneratedCodeModifier.ToString().ToLower() + " class " + this.GrammarName + "_Parser : ");
            // TODO: not nice!!!
            if (this.Grammar is CFGrammarText)
                stream.WriteLine("LR1TextParser");
            else
                stream.WriteLine("LR1BinaryParser");
            stream.WriteLine("    {");
            Export_Variables(stream);
            foreach (CFRule rule in this.GrammarRules)
                Export_Production(rule);
            Export_Rules();
            Export_States();
            Export_Actions();
            Export_Setup();
            return base.Export(expected, options);
        }
        protected void Export_Setup()
        {
            stream.WriteLine("        protected override void setup()");
            stream.WriteLine("        {");
            stream.WriteLine("            rules = staticRules;");
            stream.WriteLine("            states = staticStates;");
            stream.WriteLine("            errorSimulationLength = 3;");
            stream.WriteLine("        }");
        }
        
        protected void Export_Actions()
        {
            List<string> Names = new List<string>();
            foreach (Action action in this.Grammar.Actions)
                if (!Names.Contains(action.LocalName))
                    Names.Add(action.LocalName);

            if (Names.Count != 0)
            {
                stream.WriteLine("        public interface Actions");
                stream.WriteLine("        {");
                foreach (string name in Names)
                    stream.WriteLine("            void " + name + "(SyntaxTreeNode SubRoot);");
                stream.WriteLine("        }");
            }
        }
        protected void Export_Production(CFRule rule)
        {
            int length = rule.Definition.GetChoiceAtIndex(0).Length;
            stream.WriteLine("        private static SyntaxTreeNode Production_" + rule.Variable.SID.ToString("X") + "_" + rule.ID.ToString("X") + " (LRParser baseParser)");
            stream.WriteLine("        {");
            stream.WriteLine("            " + this.GrammarName + "_Parser parser = baseParser as " + this.GrammarName + "_Parser;");
            if (length != 0)
            {
                stream.WriteLine("            LinkedListNode<SyntaxTreeNode> current = parser.nodes.Last;");
                stream.WriteLine("            LinkedListNode<SyntaxTreeNode> temp = null;");
                for (int i = 1; i != length; i++)
                    stream.WriteLine("            current = current.Previous;");
            }
            stream.Write("            SyntaxTreeNode root = new SyntaxTreeNode(variables[" + this.variables.IndexOf(rule.Variable) + "]");
            if (rule.ReplaceOnProduction)
                stream.WriteLine(", SyntaxTreeNodeAction.Replace);");
            else
                stream.WriteLine(");");

            foreach (RuleDefinitionPart part in rule.Definition.Parts)
            {
                if (part.Symbol is Action)
                {
                    Action action = (Action)part.Symbol;
                    stream.WriteLine("            parser.actions." + action.LocalName + "(root);");
                }
                else if (part.Symbol is Virtual)
                    stream.WriteLine("            root.AppendChild(new SyntaxTreeNode(new SymbolVirtual(\"" + part.Symbol.LocalName + "\"), SyntaxTreeNodeAction." + part.Action.ToString() + "));");
                else if (part.Symbol is Terminal || part.Symbol is Variable)
                {
                    if (part.Action != RuleDefinitionPartAction.Drop)
                    {
                        stream.Write("            root.AppendChild(current.Value");
                        if (part.Action != RuleDefinitionPartAction.Nothing)
                            stream.WriteLine(", SyntaxTreeNodeAction." + part.Action.ToString() + ");");
                        else
                            stream.WriteLine(");");
                    }
                    stream.WriteLine("            temp = current.Next;");
                    stream.WriteLine("            parser.nodes.Remove(current);");
                    stream.WriteLine("            current = temp;");
                }
            }
            stream.WriteLine("            return root;");
            stream.WriteLine("        }");
        }
        protected void Export_Rules()
        {
            stream.WriteLine("        private static LRRule[] staticRules = {");
            bool first = true;
            foreach (CFRule Rule in this.GrammarRules)
            {
                stream.Write("           ");
                if (!first) stream.Write(", ");
                string production = "Production_" + Rule.Variable.SID.ToString("X") + "_" + Rule.ID.ToString("X");
                string head = "variables[" + this.variables.IndexOf(Rule.Variable) + "]";
                stream.WriteLine("new LRRule(" + production + ", " + head + ", " + Rule.Definition.GetChoiceAtIndex(0).Length + ")");
                first = false;
            }
            stream.WriteLine("        };");
        }
        
        protected void Export_State(State state)
        {
            TerminalSet expected = state.Reductions.ExpectedTerminals;
            foreach (Symbol Symbol in state.Children.Keys)
            {
                if (Symbol is Terminal)
                    expected.Add((Terminal)Symbol);
            }
            bool first = true;
            stream.WriteLine("new LR1State(");
            // Write items
            if (debug)
            {
                stream.Write("               new string[" + state.Items.Count + "] {");
                first = true;
                foreach (Item item in state.Items)
                {
                    if (!first) stream.Write(", ");
                    stream.Write("\"" + item.ToString(true) + "\"");
                    first = false;
                }
                stream.WriteLine("},");
            }
            else
            {
                stream.WriteLine("               null,");
            }
            // Write terminals
            stream.Write("               new SymbolTerminal[" + expected.Count + "] {");
            first = true;
            foreach (Terminal terminal in expected)
            {
                int index = terminals.IndexOf(terminal);
                if (index == -1)
                    reporter.Error("Grammar", "In state " + state.ID.ToString("X") + " expected terminal " + terminal.ToString() + " cannot be produced by the parser. Check the regular expressions.");
                if (!first) stream.Write(", ");
                stream.Write(terminalsAccessor + "[" + index + "]");
                first = false;
            }
            stream.WriteLine("},");

            int ShitTerminalCount = 0;
            foreach (Symbol Symbol in state.Children.Keys)
            {
                if (!(Symbol is Terminal))
                    continue;
                ShitTerminalCount++;
            }

            // Write shifts on terminal
            stream.Write("               new ushort[" + ShitTerminalCount + "] {");
            first = true;
            foreach (Symbol Symbol in state.Children.Keys)
            {
                if (!(Symbol is Terminal))
                    continue;
                if (!first) stream.Write(", ");
                stream.Write("0x" + Symbol.SID.ToString("x"));
                first = false;
            }
            stream.WriteLine("},");
            stream.Write("               new ushort[" + ShitTerminalCount + "] {");
            first = true;
            foreach (Symbol Symbol in state.Children.Keys)
            {
                if (!(Symbol is Terminal))
                    continue;
                if (!first) stream.Write(", ");
                stream.Write("0x" + state.Children[Symbol].ID.ToString("X"));
                first = false;
            }
            stream.WriteLine("},");

            // Write shifts on variable
            stream.Write("               new ushort[" + (state.Children.Count - ShitTerminalCount).ToString() + "] {");
            first = true;
            foreach (Symbol Symbol in state.Children.Keys)
            {
                if (!(Symbol is Variable))
                    continue;
                if (!first) stream.Write(", ");
                stream.Write("0x" + Symbol.SID.ToString("x"));
                first = false;
            }
            stream.WriteLine("},");
            stream.Write("               new ushort[" + (state.Children.Count - ShitTerminalCount).ToString() + "] {");
            first = true;
            foreach (Symbol Symbol in state.Children.Keys)
            {
                if (!(Symbol is Variable))
                    continue;
                if (!first) stream.Write(", ");
                stream.Write("0x" + state.Children[Symbol].ID.ToString("X"));
                first = false;
            }
            stream.WriteLine("},");
            // Write reductions
            stream.Write("               new LRReduction[" + state.Reductions.Count + "] {");
            first = true;
            foreach (StateActionReduce Reduction in state.Reductions)
            {
                if (!first) stream.Write(", ");
                stream.Write("new LRReduction(0x" + Reduction.OnSymbol.SID.ToString("x") + ", staticRules[0x" + this.GrammarRules.IndexOf(Reduction.ToReduceRule).ToString("X") + "])");
                first = false;
            }
            stream.WriteLine("})");
        }
        protected void Export_States()
        {
            stream.WriteLine("        private static LR1State[] staticStates = {");
            bool first = true;
            foreach (State State in graph.States)
            {
                stream.Write("            ");
                if (!first) stream.Write(", ");
                Export_State(State);
                first = false;
            }
            stream.WriteLine("        };");
        }
    }
}
