﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:22
 * 
 */
using System.Collections.Generic;

namespace Hime.Parsers.ContextFree.LR
{
    class StateReductionsLR1 : StateReductions
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

        public StateReductionsLR1() : base() { }

        public override void Build(State state)
        {
            // Recutions dictionnary for the given set
            Dictionary<Terminal, ItemLR1> Reductions = new Dictionary<Terminal, ItemLR1>();
            // Construct reductions
            foreach (ItemLR1 Item in state.Items)
            {
                if (Item.Action == ItemAction.Shift)
                    continue;
                // There is already a shift action for the lookahead => conflict
                if (state.Children.ContainsKey(Item.Lookahead)) HandleConflict_ShiftReduce("LR(1)", conflicts, Item, state, Item.Lookahead);
                // There is already a reduction action for the lookahead => conflict
                else if (Reductions.ContainsKey(Item.Lookahead)) HandleConflict_ReduceReduce("LR(1)", conflicts, Item, Reductions[Item.Lookahead], state, Item.Lookahead);
                else // No conflict
                {
                    Reductions.Add(Item.Lookahead, Item);
                    this.Add(new StateActionReduce(Item.Lookahead, Item.BaseRule));
                }
            }
        }

        public static void HandleConflict_ShiftReduce(string component, List<Conflict> Conflicts, Item ConflictuousItem, State Set, Terminal Lookahead)
        {
            // Look for previous conflict
            foreach (Conflict Previous in Conflicts)
            {
                if (Previous.ConflictType == ConflictType.ShiftReduce && Previous.ConflictSymbol == Lookahead)
                {
                    // Previous conflict
                    Previous.AddItem(ConflictuousItem);
                    return;
                }
            }
            // No previous conflict was found
            Conflict Conflict = new Conflict(component, Set, ConflictType.ShiftReduce, Lookahead);
            foreach (Item Item in Set.Items)
                if (Item.Action == ItemAction.Shift && Item.NextSymbol.SID == Lookahead.SID)
                    Conflict.AddItem(Item);
            Conflict.AddItem(ConflictuousItem);
            Conflicts.Add(Conflict);
        }

        public static void HandleConflict_ReduceReduce(string component, List<Conflict> Conflicts, Item ConflictuousItem, Item PreviousItem, State Set, Terminal Lookahead)
        {
            // Look for previous conflict
            foreach (Conflict Previous in Conflicts)
            {
                if (Previous.ConflictType == ConflictType.ReduceReduce && Previous.ConflictSymbol == Lookahead)
                {
                    // Previous conflict
                    Previous.AddItem(ConflictuousItem);
                    return;
                }
            }
            // No previous conflict was found
            Conflict Conflict = new Conflict(component, Set, ConflictType.ReduceReduce, Lookahead);
            Conflict.AddItem(PreviousItem);
            Conflict.AddItem(ConflictuousItem);
            Conflicts.Add(Conflict);
        }
    }
}
