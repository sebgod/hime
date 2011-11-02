﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:22
 * 
 */
using System.Collections.Generic;

namespace Hime.Parsers.ContextFree.LR
{
    class StateReductionsGLALR1 : StateReductions
    {
        public override TerminalSet ExpectedTerminals
        {
            get
            {
                TerminalSet Set = new TerminalSet();
                foreach (StateActionReduce Reduction in this)
                    Set.Add(Reduction.Lookahead);
                return Set;
            }
        }

        public StateReductionsGLALR1() : base() { }

        public override void Build(State state)
        {
            // Build shift actions
            foreach (GrammarSymbol Next in state.Children.Keys)
            {
                List<StateAction> Actions = new List<StateAction>();
                Actions.Add(new StateActionShift(Next, state.Children[Next]));
            }

            // Recutions dictionnary for the given set
            Dictionary<Terminal, ItemLALR1> Reductions = new Dictionary<Terminal, ItemLALR1>();
            // Construct reductions
            foreach (ItemLALR1 Item in state.Items)
            {
                if (Item.Action == ItemAction.Shift)
                    continue;
                foreach (Terminal Lookahead in Item.Lookaheads)
                {
                    // There is already a shift action for the lookahead => conflict
                    if (state.Children.ContainsKey(Lookahead))
                    {
                        StateReductionsLR1.HandleConflict_ShiftReduce("GLALR(1)", conflicts, Item, state, Lookahead);
                        StateActionReduce Reduction = new StateActionReduce(Lookahead, Item.BaseRule);
                        this.Add(Reduction);
                    }
                    // There is already a reduction action for the lookahead => conflict
                    else if (Reductions.ContainsKey(Lookahead))
                    {
                        StateReductionsLR1.HandleConflict_ReduceReduce("GLALR(1)", conflicts, Item, Reductions[Lookahead], state, Lookahead);
                        StateActionReduce Reduction = new StateActionReduce(Lookahead, Item.BaseRule);
                        this.Add(Reduction);
                    }
                    // No conflict
                    else
                    {
                        Reductions.Add(Lookahead, Item);
                        StateActionReduce Reduction = new StateActionReduce(Lookahead, Item.BaseRule);
                        this.Add(Reduction);
                    }
                }
            }
        }
    }
}
