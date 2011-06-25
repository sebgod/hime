﻿using System.Collections.Generic;

namespace Analyser
{
    class Test2_Lexer : Hime.Redist.Parsers.LexerText
    {
        private static ushort[] staticSymbolsSID = { 0x6, 0x7, 0x9, 0xA, 0xB };
        private static string[] staticSymbolsName = { "_T[x]", "_T[.]", "_T[(]", "_T[)]", "_T[typeof]" };
        private static ushort[][] staticTransitions0 = { new ushort[3] { 0x78, 0x78, 0x6 }, new ushort[3] { 0x2E, 0x2E, 0x7 }, new ushort[3] { 0x28, 0x28, 0x8 }, new ushort[3] { 0x29, 0x29, 0x9 }, new ushort[3] { 0x74, 0x74, 0x1 } };
        private static ushort[][] staticTransitions1 = { new ushort[3] { 0x79, 0x79, 0x2 } };
        private static ushort[][] staticTransitions2 = { new ushort[3] { 0x70, 0x70, 0x3 } };
        private static ushort[][] staticTransitions3 = { new ushort[3] { 0x65, 0x65, 0x4 } };
        private static ushort[][] staticTransitions4 = { new ushort[3] { 0x6F, 0x6F, 0x5 } };
        private static ushort[][] staticTransitions5 = { new ushort[3] { 0x66, 0x66, 0xA } };
        private static ushort[][] staticTransitions6 = {  };
        private static ushort[][] staticTransitions7 = {  };
        private static ushort[][] staticTransitions8 = {  };
        private static ushort[][] staticTransitions9 = {  };
        private static ushort[][] staticTransitionsA = {  };
        private static ushort[][][] staticTransitions = { staticTransitions0, staticTransitions1, staticTransitions2, staticTransitions3, staticTransitions4, staticTransitions5, staticTransitions6, staticTransitions7, staticTransitions8, staticTransitions9, staticTransitionsA };
        private static int[] staticFinals = { -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4 };
        protected override void setup() {
            symbolsSID = staticSymbolsSID;
            symbolsName = staticSymbolsName;
            symbolsSubGrammars = new Dictionary<ushort, MatchSubGrammar>();
            transitions = staticTransitions;
            finals = staticFinals;
        }
        public override Hime.Redist.Parsers.ILexer Clone() {
            return new Test2_Lexer(this);
        }
        public Test2_Lexer(string input) : base(new System.IO.StringReader(input)) {}
        public Test2_Lexer(System.IO.TextReader input) : base(input) {}
        public Test2_Lexer(Test2_Lexer original) : base(original) {}
    }
    class Test2_Parser : Hime.Redist.Parsers.BaseLRStarParser
    {
        private static Hime.Redist.Parsers.SymbolTerminal[] staticTerminals = {
            new Hime.Redist.Parsers.SymbolTerminal("ε", 0x1)
            , new Hime.Redist.Parsers.SymbolTerminal("$", 0x2)
            , new Hime.Redist.Parsers.SymbolTerminal("_T[x]", 0x6)
            , new Hime.Redist.Parsers.SymbolTerminal("_T[.]", 0x7)
            , new Hime.Redist.Parsers.SymbolTerminal("_T[(]", 0x9)
            , new Hime.Redist.Parsers.SymbolTerminal("_T[)]", 0xA)
            , new Hime.Redist.Parsers.SymbolTerminal("_T[typeof]", 0xB)
        };
        private static Hime.Redist.Parsers.SymbolVariable[] staticVariables = {
            new Hime.Redist.Parsers.SymbolVariable(0x3, "e0")
            , new Hime.Redist.Parsers.SymbolVariable(0x4, "e")
            , new Hime.Redist.Parsers.SymbolVariable(0x5, "t")
            , new Hime.Redist.Parsers.SymbolVariable(0x13, "_Axiom_")
        };
        private static void Production_3_0 (Hime.Redist.Parsers.BaseLRStarParser parser, List<Hime.Redist.Parsers.SyntaxTreeNode> nodes)
        {
            List<Hime.Redist.Parsers.SyntaxTreeNode> Definition = nodes.GetRange(nodes.Count - 1, 1);
            nodes.RemoveRange(nodes.Count - 1, 1);
            Hime.Redist.Parsers.SyntaxTreeNode SubRoot = new Hime.Redist.Parsers.SyntaxTreeNode(staticVariables[0]);
            SubRoot.AppendChild(Definition[0]);
            nodes.Add(SubRoot);
        }
        private static void Production_3_1 (Hime.Redist.Parsers.BaseLRStarParser parser, List<Hime.Redist.Parsers.SyntaxTreeNode> nodes)
        {
            List<Hime.Redist.Parsers.SyntaxTreeNode> Definition = nodes.GetRange(nodes.Count - 3, 3);
            nodes.RemoveRange(nodes.Count - 3, 3);
            Hime.Redist.Parsers.SyntaxTreeNode SubRoot = new Hime.Redist.Parsers.SyntaxTreeNode(staticVariables[0]);
            SubRoot.AppendChild(Definition[0]);
            SubRoot.AppendChild(Definition[1]);
            SubRoot.AppendChild(Definition[2]);
            nodes.Add(SubRoot);
        }
        private static void Production_3_2 (Hime.Redist.Parsers.BaseLRStarParser parser, List<Hime.Redist.Parsers.SyntaxTreeNode> nodes)
        {
            List<Hime.Redist.Parsers.SyntaxTreeNode> Definition = nodes.GetRange(nodes.Count - 3, 3);
            nodes.RemoveRange(nodes.Count - 3, 3);
            Hime.Redist.Parsers.SyntaxTreeNode SubRoot = new Hime.Redist.Parsers.SyntaxTreeNode(staticVariables[0]);
            SubRoot.AppendChild(Definition[0]);
            SubRoot.AppendChild(Definition[1]);
            SubRoot.AppendChild(Definition[2]);
            nodes.Add(SubRoot);
        }
        private static void Production_3_3 (Hime.Redist.Parsers.BaseLRStarParser parser, List<Hime.Redist.Parsers.SyntaxTreeNode> nodes)
        {
            List<Hime.Redist.Parsers.SyntaxTreeNode> Definition = nodes.GetRange(nodes.Count - 4, 4);
            nodes.RemoveRange(nodes.Count - 4, 4);
            Hime.Redist.Parsers.SyntaxTreeNode SubRoot = new Hime.Redist.Parsers.SyntaxTreeNode(staticVariables[0]);
            SubRoot.AppendChild(Definition[0]);
            SubRoot.AppendChild(Definition[1]);
            SubRoot.AppendChild(Definition[2]);
            SubRoot.AppendChild(Definition[3]);
            nodes.Add(SubRoot);
        }
        private static void Production_4_0 (Hime.Redist.Parsers.BaseLRStarParser parser, List<Hime.Redist.Parsers.SyntaxTreeNode> nodes)
        {
            List<Hime.Redist.Parsers.SyntaxTreeNode> Definition = nodes.GetRange(nodes.Count - 1, 1);
            nodes.RemoveRange(nodes.Count - 1, 1);
            Hime.Redist.Parsers.SyntaxTreeNode SubRoot = new Hime.Redist.Parsers.SyntaxTreeNode(staticVariables[1]);
            SubRoot.AppendChild(Definition[0]);
            nodes.Add(SubRoot);
        }
        private static void Production_4_1 (Hime.Redist.Parsers.BaseLRStarParser parser, List<Hime.Redist.Parsers.SyntaxTreeNode> nodes)
        {
            List<Hime.Redist.Parsers.SyntaxTreeNode> Definition = nodes.GetRange(nodes.Count - 4, 4);
            nodes.RemoveRange(nodes.Count - 4, 4);
            Hime.Redist.Parsers.SyntaxTreeNode SubRoot = new Hime.Redist.Parsers.SyntaxTreeNode(staticVariables[1]);
            SubRoot.AppendChild(Definition[0]);
            SubRoot.AppendChild(Definition[1]);
            SubRoot.AppendChild(Definition[2]);
            SubRoot.AppendChild(Definition[3]);
            nodes.Add(SubRoot);
        }
        private static void Production_5_0 (Hime.Redist.Parsers.BaseLRStarParser parser, List<Hime.Redist.Parsers.SyntaxTreeNode> nodes)
        {
            List<Hime.Redist.Parsers.SyntaxTreeNode> Definition = nodes.GetRange(nodes.Count - 1, 1);
            nodes.RemoveRange(nodes.Count - 1, 1);
            Hime.Redist.Parsers.SyntaxTreeNode SubRoot = new Hime.Redist.Parsers.SyntaxTreeNode(staticVariables[2]);
            SubRoot.AppendChild(Definition[0]);
            nodes.Add(SubRoot);
        }
        private static void Production_5_1 (Hime.Redist.Parsers.BaseLRStarParser parser, List<Hime.Redist.Parsers.SyntaxTreeNode> nodes)
        {
            List<Hime.Redist.Parsers.SyntaxTreeNode> Definition = nodes.GetRange(nodes.Count - 3, 3);
            nodes.RemoveRange(nodes.Count - 3, 3);
            Hime.Redist.Parsers.SyntaxTreeNode SubRoot = new Hime.Redist.Parsers.SyntaxTreeNode(staticVariables[2]);
            SubRoot.AppendChild(Definition[0]);
            SubRoot.AppendChild(Definition[1]);
            SubRoot.AppendChild(Definition[2]);
            nodes.Add(SubRoot);
        }
        private static void Production_13_0 (Hime.Redist.Parsers.BaseLRStarParser parser, List<Hime.Redist.Parsers.SyntaxTreeNode> nodes)
        {
            List<Hime.Redist.Parsers.SyntaxTreeNode> Definition = nodes.GetRange(nodes.Count - 2, 2);
            nodes.RemoveRange(nodes.Count - 2, 2);
            Hime.Redist.Parsers.SyntaxTreeNode SubRoot = new Hime.Redist.Parsers.SyntaxTreeNode(staticVariables[3]);
            SubRoot.AppendChild(Definition[0], Hime.Redist.Parsers.SyntaxTreeNodeAction.Promote);
            SubRoot.AppendChild(Definition[1], Hime.Redist.Parsers.SyntaxTreeNodeAction.Drop);
            nodes.Add(SubRoot);
        }
        private static Rule[] staticRules = {
           new Rule(Production_3_0, staticVariables[0], 1)
           , new Rule(Production_3_1, staticVariables[0], 3)
           , new Rule(Production_3_2, staticVariables[0], 3)
           , new Rule(Production_3_3, staticVariables[0], 4)
           , new Rule(Production_4_0, staticVariables[1], 1)
           , new Rule(Production_4_1, staticVariables[1], 4)
           , new Rule(Production_5_0, staticVariables[2], 1)
           , new Rule(Production_5_1, staticVariables[2], 3)
           , new Rule(Production_13_0, staticVariables[3], 2)
        };
        private static State[] staticStates = {
            new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[3] {staticTerminals[2], staticTerminals[4], staticTerminals[6]},
               new DeciderState[4] {
                   new DeciderState(
                   new ushort[3] {0x9, 0x6, 0xB},
                   new ushort[3] {0x1, 0x2, 0x3}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0x3, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0x4, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0x5, new Rule())
               },
               new ushort[2] {0x4, 0x3},
               new ushort[2] {0x1, 0x2})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[1] {staticTerminals[1]},
               new DeciderState[2] {
                   new DeciderState(
                   new ushort[1] {0x2},
                   new ushort[1] {0x1}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0x6, new Rule())
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[3] {staticTerminals[1], staticTerminals[3], staticTerminals[5]},
               new DeciderState[3] {
                   new DeciderState(
                   new ushort[3] {0x2, 0xA, 0x7},
                   new ushort[3] {0x1, 0x1, 0x2}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xFFFF, staticRules[0x4])
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0x7, new Rule())
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[3] {staticTerminals[2], staticTerminals[4], staticTerminals[6]},
               new DeciderState[4] {
                   new DeciderState(
                   new ushort[3] {0x6, 0x9, 0xB},
                   new ushort[3] {0x1, 0x2, 0x3}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xA, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0x3, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0x5, new Rule())
               },
               new ushort[3] {0x5, 0x4, 0x3},
               new ushort[3] {0x8, 0x9, 0x2})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[3] {staticTerminals[1], staticTerminals[3], staticTerminals[5]},
               new DeciderState[2] {
                   new DeciderState(
                   new ushort[3] {0x2, 0x7, 0xA},
                   new ushort[3] {0x1, 0x1, 0x1}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xFFFF, staticRules[0x0])
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[1] {staticTerminals[4]},
               new DeciderState[2] {
                   new DeciderState(
                   new ushort[1] {0x9},
                   new ushort[1] {0x1}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xB, new Rule())
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[1] {staticTerminals[0]},
               new DeciderState[2] {
                   new DeciderState(
                   new ushort[1] {0x1},
                   new ushort[1] {0x1}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xFFFF, staticRules[0x8])
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[1] {staticTerminals[2]},
               new DeciderState[2] {
                   new DeciderState(
                   new ushort[1] {0x6},
                   new ushort[1] {0x1}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xC, new Rule())
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[2] {staticTerminals[3], staticTerminals[5]},
               new DeciderState[3] {
                   new DeciderState(
                   new ushort[2] {0xA, 0x7},
                   new ushort[2] {0x1, 0x2}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xD, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xE, new Rule())
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[1] {staticTerminals[5]},
               new DeciderState[2] {
                   new DeciderState(
                   new ushort[1] {0xA},
                   new ushort[1] {0x1}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xF, new Rule())
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[2] {staticTerminals[3], staticTerminals[5]},
               new DeciderState[6] {
                   new DeciderState(
                   new ushort[2] {0x7, 0xA},
                   new ushort[2] {0x1, 0x2}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[1] {0x6},
                   new ushort[1] {0x3}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[6] {0x9, 0x6, 0xB, 0x2, 0x7, 0xA},
                   new ushort[6] {0x4, 0x4, 0x4, 0x5, 0x5, 0x5}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[3] {0x2, 0x7, 0xA},
                   new ushort[3] {0x5, 0x1, 0x2}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xFFFF, staticRules[0x6])
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xFFFF, staticRules[0x0])
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[1] {staticTerminals[2]},
               new DeciderState[2] {
                   new DeciderState(
                   new ushort[1] {0x6},
                   new ushort[1] {0x1}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0x11, new Rule())
               },
               new ushort[1] {0x5},
               new ushort[1] {0x10})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[3] {staticTerminals[1], staticTerminals[3], staticTerminals[5]},
               new DeciderState[2] {
                   new DeciderState(
                   new ushort[3] {0x2, 0x7, 0xA},
                   new ushort[3] {0x1, 0x1, 0x1}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xFFFF, staticRules[0x1])
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[3] {staticTerminals[2], staticTerminals[4], staticTerminals[6]},
               new DeciderState[4] {
                   new DeciderState(
                   new ushort[3] {0x9, 0x6, 0xB},
                   new ushort[3] {0x1, 0x2, 0x3}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0x3, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0x4, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0x5, new Rule())
               },
               new ushort[2] {0x4, 0x3},
               new ushort[2] {0x12, 0x2})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[1] {staticTerminals[2]},
               new DeciderState[2] {
                   new DeciderState(
                   new ushort[1] {0x6},
                   new ushort[1] {0x1}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0x13, new Rule())
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[3] {staticTerminals[1], staticTerminals[3], staticTerminals[5]},
               new DeciderState[2] {
                   new DeciderState(
                   new ushort[3] {0x2, 0x7, 0xA},
                   new ushort[3] {0x1, 0x1, 0x1}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xFFFF, staticRules[0x2])
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[2] {staticTerminals[3], staticTerminals[5]},
               new DeciderState[3] {
                   new DeciderState(
                   new ushort[2] {0xA, 0x7},
                   new ushort[2] {0x1, 0x2}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0x14, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xE, new Rule())
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[2] {staticTerminals[3], staticTerminals[5]},
               new DeciderState[2] {
                   new DeciderState(
                   new ushort[2] {0x7, 0xA},
                   new ushort[2] {0x1, 0x1}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xFFFF, staticRules[0x6])
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[2] {staticTerminals[1], staticTerminals[5]},
               new DeciderState[2] {
                   new DeciderState(
                   new ushort[2] {0x2, 0xA},
                   new ushort[2] {0x1, 0x1}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xFFFF, staticRules[0x5])
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[2] {staticTerminals[3], staticTerminals[5]},
               new DeciderState[2] {
                   new DeciderState(
                   new ushort[2] {0x7, 0xA},
                   new ushort[2] {0x1, 0x1}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xFFFF, staticRules[0x7])
               },
               new ushort[0] {},
               new ushort[0] {})
            , new State(
               null,
               new Hime.Redist.Parsers.SymbolTerminal[3] {staticTerminals[1], staticTerminals[3], staticTerminals[5]},
               new DeciderState[2] {
                   new DeciderState(
                   new ushort[3] {0x2, 0x7, 0xA},
                   new ushort[3] {0x1, 0x1, 0x1}, 0xFFFF, new Rule())
                   , new DeciderState(
                   new ushort[0] {},
                   new ushort[0] {}, 0xFFFF, staticRules[0x3])
               },
               new ushort[0] {},
               new ushort[0] {})
        };
        protected override void setup()
        {
            rules = staticRules;
            states = staticStates;
            errorSimulationLength = 3;
        }
        public Test2_Parser(Test2_Lexer lexer) : base (lexer) {}
    }
}
