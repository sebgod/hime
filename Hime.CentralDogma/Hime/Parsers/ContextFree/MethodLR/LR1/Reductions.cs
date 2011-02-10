﻿namespace Hime.Parsers.CF.LR
{
    /// <summary>
    /// Represents the actions for a LR(1) set
    /// </summary>
    internal class ItemSetReductionsLR1 : ItemSetReductions
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
        public ItemSetReductionsLR1() : base()
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
            System.Collections.Generic.Dictionary<Terminal, ItemLR1> Reductions = new System.Collections.Generic.Dictionary<Terminal, ItemLR1>();
            // Construct reductions
            foreach (ItemLR1 Item in Set.Items)
            {
                if (Item.Action == ItemAction.Shift)
                    continue;
                // There is already a shift action for the lookahead => conflict
                if (Set.Children.ContainsKey(Item.Lookahead)) HandleConflict_ShiftReduce(p_Conflicts, Item, Set, Item.Lookahead);
                // There is already a reduction action for the lookahead => conflict
                else if (Reductions.ContainsKey(Item.Lookahead)) HandleConflict_ReduceReduce(p_Conflicts, Item, Reductions[Item.Lookahead], Set, Item.Lookahead);
                else // No conflict
                {
                    Reductions.Add(Item.Lookahead, Item);
                    p_ActionReductions.Add(new ItemSetActionReduce(Item.Lookahead, Item.BaseRule));
                }
            }
        }

        /// <summary>
        /// Handle a LR(1) Shift/Reduce conflict
        /// </summary>
        /// <param name="Conflicts">List of the previous conflicts</param>
        /// <param name="ConflictuousItem">New conflictuous item</param>
        /// <param name="Set">Set containing the conflictuous items</param>
        /// <param name="OnSymbol">OnSymbol provoking the conflict</param>
        public static void HandleConflict_ShiftReduce(System.Collections.Generic.List<Conflict> Conflicts, Item ConflictuousItem, ItemSet Set, Terminal Lookahead)
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
            Conflict Conflict = new Conflict(typeof(MethodLR1), ConflictType.ShiftReduce, Lookahead);
            foreach (Item Item in Set.Items)
                if (Item.Action == ItemAction.Shift && Item.NextSymbol.SID == Lookahead.SID)
                    Conflict.AddItem(Item);
            Conflict.AddItem(ConflictuousItem);
            Conflicts.Add(Conflict);
        }

        /// <summary>
        /// Handle a LR(1) Reduce/Reduce conflict
        /// </summary>
        /// <param name="Conflicts">List of the previous conflicts</param>
        /// <param name="ConflictuousItem">New conflictuous item</param>
        /// <param name="PreviousItem">Previous item with the reduction action</param>
        /// <param name="Set">Set containing the conflictuous items</param>
        /// <param name="OnSymbol">OnSymbol provoking the conflict</param>
        public static void HandleConflict_ReduceReduce(System.Collections.Generic.List<Conflict> Conflicts, Item ConflictuousItem, Item PreviousItem, ItemSet Set, Terminal Lookahead)
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
            Conflict Conflict = new Conflict(typeof(MethodLR1), ConflictType.ReduceReduce, Lookahead);
            Conflict.AddItem(PreviousItem);
            Conflict.AddItem(ConflictuousItem);
            Conflicts.Add(Conflict);
        }
    }
}