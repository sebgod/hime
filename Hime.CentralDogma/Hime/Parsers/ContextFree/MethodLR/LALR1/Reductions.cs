﻿namespace Hime.Parsers.CF.LR
{
    /// <summary>
    /// Represents the actions for a LALR(1) set
    /// </summary>
    internal class ItemSetReductionsLALR1 : ItemSetReductions
    {
        /// <summary>
        /// Reduction actions
        /// </summary>
        private System.Collections.Generic.List<ItemSetActionReduce> p_ActionReductions;

        public override System.Collections.Generic.IEnumerable<ItemSetAction> Actions
        {
            get
            {
                System.Collections.Generic.List<ItemSetAction> Result = new System.Collections.Generic.List<ItemSetAction>();
                foreach (ItemSetActionReduce action in p_ActionReductions)
                    Result.Add(action);
                return Result;
            }
        }
        public override TerminalSet ExpectedTerminals
        {
            get
            {
                TerminalSet Set = new TerminalSet();
                foreach (ItemSetActionReduce Reduction in p_ActionReductions)
                    Set.Add(Reduction.Lookahead);
                return Set;
            }
        }

        /// <summary>
        /// Constructs the actions
        /// </summary>
        public ItemSetReductionsLALR1() : base()
        {
            p_ActionReductions = new System.Collections.Generic.List<ItemSetActionReduce>();
        }

        /// <summary>
        /// Build the actions for the given set of items
        /// </summary>
        /// <param name="Set">The set of items</param>
        public override void Build(ItemSet Set)
        {
            // Recutions dictionnary for the given set
            System.Collections.Generic.Dictionary<Terminal, ItemLALR1> Reductions = new System.Collections.Generic.Dictionary<Terminal, ItemLALR1>();
            // Construct reductions
            foreach (ItemLALR1 Item in Set.Items)
            {
                if (Item.Action == ItemAction.Shift)
                    continue;
                foreach (Terminal Lookahead in Item.Lookaheads)
                {
                    // There is already a shift action for the lookahead => conflict
                    if (Set.Children.ContainsKey(Lookahead))
                        ItemSetReductionsLR1.HandleConflict_ShiftReduce(p_Conflicts, Item, Set, Lookahead);
                    // There is already a reduction action for the lookahead => conflict
                    else if (Reductions.ContainsKey(Lookahead))
                        ItemSetReductionsLR1.HandleConflict_ReduceReduce(p_Conflicts, Item, Reductions[Lookahead], Set, Lookahead);
                    else // No conflict
                    {
                        Reductions.Add(Lookahead, Item);
                        p_ActionReductions.Add(new ItemSetActionReduce(Lookahead, Item.BaseRule));
                    }
                }
            }
        }
    }
}