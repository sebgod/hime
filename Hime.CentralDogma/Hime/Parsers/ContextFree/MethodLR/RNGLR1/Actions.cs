﻿namespace Hime.Parsers.CF.LR
{
    /// <summary>
    /// Represents the actions for a RNGLR(1) set
    /// </summary>
    internal class ItemSetReductionsRNGLR1 : ItemSetReductions
    {
        /// <summary>
        /// Reduction actions
        /// </summary>
        private System.Collections.Generic.List<ItemSetActionRNReduce> p_ActionReductions;

        public override System.Collections.Generic.IEnumerable<ItemSetActionReduce> Reductions {
            get
            {
                System.Collections.Generic.List<ItemSetActionReduce> Temp = new System.Collections.Generic.List<ItemSetActionReduce>();
                foreach (ItemSetActionRNReduce action in p_ActionReductions)
                    Temp.Add(action);
                return Temp;
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
        public ItemSetReductionsRNGLR1()
        {
            p_ActionReductions = new System.Collections.Generic.List<ItemSetActionRNReduce>();
        }

        /// <summary>
        /// Build the actions for the given set of items
        /// </summary>
        /// <param name="Set">The set of items</param>
        public override void Build(ItemSet Set)
        {
            // Build shift actions
            foreach (Symbol Next in Set.Children.Keys)
            {
                System.Collections.Generic.List<ItemSetAction> Actions = new System.Collections.Generic.List<ItemSetAction>();
                Actions.Add(new ItemSetActionShift(Next, Set.Children[Next]));
            }

            // Redutions dictionnary for the given set
            System.Collections.Generic.Dictionary<Terminal, ItemLR1> Reductions = new System.Collections.Generic.Dictionary<Terminal, ItemLR1>();
            // Construct reductions
            foreach (ItemLR1 Item in Set.Items)
            {
                // Check for right nulled reduction
                if (Item.Action == ItemAction.Shift && !Item.BaseRule.Definition.GetChoiceAtIndex(Item.DotPosition).Firsts.Contains(TerminalEpsilon.Instance))
                    continue;
                // There is already a shift action for the lookahead => conflict
                if (Set.Children.ContainsKey(Item.Lookahead))
                {
                    ItemSetReductionsLR1.HandleConflict_ShiftReduce(typeof(MethodRNGLR1), p_Conflicts, Item, Set, Item.Lookahead);
                    ItemSetActionRNReduce Reduction = new ItemSetActionRNReduce(Item.Lookahead, Item.BaseRule, Item.DotPosition);
                    p_ActionReductions.Add(Reduction);
                }
                // There is already a reduction action for the lookahead => conflict
                else if (Reductions.ContainsKey(Item.Lookahead))
                {
                    ItemSetReductionsLR1.HandleConflict_ReduceReduce(typeof(MethodRNGLR1), p_Conflicts, Item, Reductions[Item.Lookahead], Set, Item.Lookahead);
                    ItemSetActionRNReduce Reduction = new ItemSetActionRNReduce(Item.Lookahead, Item.BaseRule, Item.DotPosition);
                    p_ActionReductions.Add(Reduction);
                }
                else // No conflict
                {
                    Reductions.Add(Item.Lookahead, Item);
                    ItemSetActionRNReduce Reduction = new ItemSetActionRNReduce(Item.Lookahead, Item.BaseRule, Item.DotPosition);
                    p_ActionReductions.Add(Reduction);
                }
            }
        }
    }
}