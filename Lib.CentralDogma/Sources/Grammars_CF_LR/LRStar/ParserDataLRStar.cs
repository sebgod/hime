/**********************************************************************
* Copyright (c) 2013 Laurent Wouters and others
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

using System;
using System.IO;
using System.Collections.Generic;

namespace Hime.CentralDogma.Grammars.ContextFree.LR
{
    class ParserDataLRStar : ParserDataLR
    {
		protected override string BaseClassName { get { return "LRStarParser"; } }

        private Dictionary<State, DeciderLRStar> deciders;

        public ParserDataLRStar(Reporting.Reporter reporter, CFGrammar gram, Graph graph, Dictionary<State, DeciderLRStar> deciders)
            : base(reporter, gram, graph)
        {
            this.deciders = deciders;
        }

        public override void ExportData(BinaryWriter stream)
        {
            throw new NotImplementedException();
        }

        protected override void ExportAutomaton(StreamWriter stream, string name, string className)
        {
        }

        /*protected override void ExportSetup(StreamWriter stream)
        {
            stream.WriteLine("        protected override void setup()");
            stream.WriteLine("        {");
            stream.WriteLine("            rules = staticRules;");
            stream.WriteLine("            states = staticStates;");
            stream.WriteLine("            errorSimulationLength = 3;");
            stream.WriteLine("        }");
        }

        protected override void ExportActions(StreamWriter stream)
        {
            List<string> Names = new List<string>();
            foreach (Action action in this.GrammarActions)
                if (!Names.Contains(action.Name))
                    Names.Add(action.Name);

            if (Names.Count != 0)
            {
                stream.WriteLine("        public interface Actions");
                stream.WriteLine("        {");
                foreach (string name in Names)
                    stream.WriteLine("            void " + name + "(SyntaxTreeNode SubRoot);");
                stream.WriteLine("        }");
            }
        }
		
		// TODO: should factor this method with other ExportRules
		override protected void ExportRules(StreamWriter stream)
        {
            stream.WriteLine("        private static LRRule[] staticRules = {");
            bool first = true;
            foreach (CFRule Rule in this.GrammarRules)
            {
                stream.Write("           ");
                if (!first) stream.Write(", ");
                string production = "Production_" + Rule.Head.SID.ToString("X") + "_" + Rule.ID.ToString("X");
                string head = "variables[" + this.variables.IndexOf(Rule.Head) + "]";
                stream.WriteLine("new LRRule(" + production + ", " + head + ", " + Rule.CFBody.GetChoiceAt(0).Length + ")");
                first = false;
            }
            stream.WriteLine("        };");
        }


        protected void Export_DeciderState(StreamWriter stream, DeciderLRStar decider, DeciderStateLRStar state)
        {
            stream.WriteLine("new DeciderState(");
            // write transitions
            stream.Write("                   new ushort[" + state.Transitions.Count + "] {");
            bool first = true;
            foreach (Terminal t in state.Transitions.Keys)
            {
                if (!first) stream.Write(", ");
                stream.Write("0x" + t.SID.ToString("X"));
                first = false;
            }
            stream.WriteLine("},");
            stream.Write("                   new ushort[" + state.Transitions.Count + "] {");
            first = true;
            foreach (Terminal t in state.Transitions.Keys)
            {
                DeciderStateLRStar child = state.Transitions[t];
                if (!first) stream.Write(", ");
                stream.Write("0x" + child.ID.ToString("X"));
                first = false;
            }
            stream.Write("}");

            // write shift decision
            if (state.Decision != -1)
            {
                Item item = decider.GetItem(state.Decision);
                if (item.Action == ItemAction.Shift)
                    stream.Write(", 0x" + decider.LRState.Children[item.NextSymbol].ID.ToString("X") + ", null");
                else
                    stream.Write(", 0xFFFF, staticRules[" + this.IndexOfRule(item.BaseRule) + "]");
            }
            else
                stream.Write(", 0xFFFF, null");
            stream.WriteLine(")");
        }
        
        protected void Export_State(StreamWriter stream, State state)
        {
            TerminalSet expected = state.Reductions.ExpectedTerminals;
            foreach (GrammarSymbol Symbol in state.Children.Keys)
            {
                if (Symbol is Terminal)
                    expected.Add((Terminal)Symbol);
            }
            bool first = true;
            stream.WriteLine("new LRStarState(");
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
            stream.Write("               new Terminal[" + expected.Count + "] {");
            first = true;
            foreach (Terminal terminal in expected)
            {
                int index = terminals.IndexOf(terminal);
                if (index == -1)
                    reporter.Error("Grammar", "In state " + state.ID.ToString("X") + " expected terminal " + terminal.ToString() + " cannot be produced by the lexer. Check the regular expressions.");
                if (!first) stream.Write(", ");
                stream.Write(terminalsAccessor + "[" + index + "]");
                first = false;
            }
            stream.WriteLine("},");

            int ShitTerminalCount = 0;
            foreach (GrammarSymbol Symbol in state.Children.Keys)
            {
                if (!(Symbol is Terminal))
                    continue;
                ShitTerminalCount++;
            }
            // write submachine
            stream.WriteLine("               new DeciderState[" + deciders[state].States.Count + "] {");
            first = true;
            foreach (DeciderStateLRStar sub in deciders[state].States)
            {
                stream.Write("                   ");
                if (!first) stream.Write(", ");
                Export_DeciderState(stream, deciders[state], sub);
                first = false;
            }
            stream.WriteLine("               },");
            // Write shifts on variable
            stream.Write("               new ushort[" + (state.Children.Count - ShitTerminalCount).ToString() + "] {");
            first = true;
            foreach (GrammarSymbol Symbol in state.Children.Keys)
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
            foreach (GrammarSymbol Symbol in state.Children.Keys)
            {
                if (!(Symbol is Variable))
                    continue;
                if (!first) stream.Write(", ");
                stream.Write("0x" + state.Children[Symbol].ID.ToString("X"));
                first = false;
            }
            stream.WriteLine("})");
        }
        protected override void ExportStates(StreamWriter stream)
        {
            stream.WriteLine("        private static LRStarState[] staticStates = {");
            bool first = true;
            foreach (State State in graph.States)
            {
                stream.Write("            ");
                if (!first) stream.Write(", ");
                Export_State(stream, State);
                first = false;
            }
            stream.WriteLine("        };");
        }

        protected override void SerializeSpecifics(string directory, bool exportVisuals, string dotBin, List<string> results)
        {
            foreach (State state in deciders.Keys)
            {
                DOTSerializer serializer = new DOTSerializer("State_" + state.ID.ToString(), directory + "\\Set_" + state.ID.ToString("X") + ".dot");
                Serialize_Deciders(deciders[state], serializer);
                serializer.Close();
                results.Add(directory + "\\Set_" + state.ID.ToString("X") + ".dot");
                if (exportVisuals)
                {
                    DOTLayoutManager layout = new DOTExternalLayoutManager(dotBin);
                    layout.Render(directory + "\\Set_" + state.ID.ToString("X") + ".dot", directory + "\\Set_" + state.ID.ToString("X") + ".svg");
                    results.Add(directory + "\\Set_" + state.ID.ToString("X") + ".svg");
                }
            }
        }

        private void Serialize_Deciders(DeciderLRStar machine, DOTSerializer serializer)
        {
            foreach (DeciderStateLRStar state in machine.States)
            {
                string id = state.ID.ToString();
                string label = id;
                if (state.Decision != -1)
                {
                    Item item = machine.GetItem(state.Decision);
                    if (item.Action == ItemAction.Shift)
                        label = "SHIFT: " + machine.LRState.Children[item.NextSymbol].ID.ToString("X");
                    else
                        label = item.BaseRule.ToString();
                }
                serializer.WriteNode(id, label);
            }
            foreach (DeciderStateLRStar state in machine.States)
                foreach (Terminal t in state.Transitions.Keys)
                    serializer.WriteEdge(state.ID.ToString(), state.Transitions[t].ID.ToString(), t.ToString().Replace("\"", "\\\""));
        }*/
    }
}
