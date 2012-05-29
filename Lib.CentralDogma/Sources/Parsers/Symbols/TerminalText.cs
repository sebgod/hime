﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:22
 * 
 */
using System.Xml;

namespace Hime.Parsers
{
    class TerminalText : Terminal
    {
        public Automata.NFA NFA { get; set; }
        public string Value { get; private set; }

        public TerminalText(ushort sid, string name, string value, int priority, Automata.NFA nfa)
            : base(sid, name, priority)
        {
            this.NFA = nfa;
            this.Value = value;
        }

        public override string ToString() { return Value; }
    }
}