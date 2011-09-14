﻿namespace Hime.Parsers
{
    public sealed class TerminalEpsilon : Terminal
    {
        private static TerminalEpsilon instance;
        private static readonly object _lock = new object();
        private TerminalEpsilon() : base(null, 1, "ε", 0) { }

        public static TerminalEpsilon Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                        instance = new TerminalEpsilon();
                    return instance;
                }
            }
        }

        public override string ToString() { return "ε"; }
    }
}