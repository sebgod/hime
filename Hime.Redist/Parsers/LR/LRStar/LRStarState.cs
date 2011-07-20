﻿using System.Collections.Generic;

namespace Hime.Redist.Parsers
{
    public sealed class LRStarState : LRState
    {
        public DeciderState[] decider;
        public LRStarState(string[] items, SymbolTerminal[] expected, DeciderState[] decider, ushort[] sv_keys, ushort[] sv_val) : base(items)
        {
            this.expecteds = expected;
            this.shiftsOnTerminal = null;
            this.shiftsOnVariable = new Dictionary<ushort, ushort>();
            for (int i = 0; i != sv_keys.Length; i++)
                this.shiftsOnVariable.Add(sv_keys[i], sv_val[i]);
            this.decider = decider;
        }
    }
}