﻿namespace Hime.Parsers.CF.LR
{
    /// <summary>
    /// Represents a graph of LALR(1) kernels based on a LR(0) graph
    /// </summary>
    internal class KernelGraph
    {
        /// <summary>
        /// The LR(0) graph
        /// </summary>
        private Graph p_GraphLR0;
        /// <summary>
        /// The LALR(1) graph
        /// </summary>
        private Graph p_GraphLALR1;
        /// <summary>
        /// Dictionnary associating LR(0) sets to LALR(1) kernels
        /// </summary>
        private System.Collections.Generic.Dictionary<ItemSetKernel, ItemSet> p_KernelsToLR0;
        /// <summary>
        /// Reverse-dictionnary associating LALR(1) kernels to LR(0) sets
        /// </summary>
        private System.Collections.Generic.Dictionary<ItemSet, ItemSetKernel> p_LR0ToKernels;
        /// <summary>
        /// Lookaheads propagation table : source LALR(1) items
        /// </summary>
        private System.Collections.Generic.List<ItemLALR1> p_PropagOrigins;
        /// <summary>
        /// Lookaheads propagation table : destination LALR(1) items
        /// </summary>
        private System.Collections.Generic.List<ItemLALR1> p_PropagTargets;

        /// <summary>
        /// Constructs the graph from the given LR(0) graph
        /// </summary>
        /// <param name="GraphLR0">LR(0) graph</param>
        public KernelGraph(Graph GraphLR0)
        {
            p_GraphLR0 = GraphLR0;
            p_KernelsToLR0 = new System.Collections.Generic.Dictionary<ItemSetKernel, ItemSet>();
            p_LR0ToKernels = new System.Collections.Generic.Dictionary<ItemSet, ItemSetKernel>();
            p_PropagOrigins = new System.Collections.Generic.List<ItemLALR1>();
            p_PropagTargets = new System.Collections.Generic.List<ItemLALR1>();
        }

        /// <summary>
        /// Create the LALR(1) kernels from the LR(0) sets
        /// </summary>
        private void BuildKernels()
        {
            for (int i = 0; i != p_GraphLR0.Sets.Count; i++)
            {
                ItemSet SetLR0 = p_GraphLR0.Sets[i];
                ItemSetKernel KernelLALR1 = new ItemSetKernel();
                foreach (Item Item in SetLR0.Kernel.Items)
                {
                    ItemLALR1 ItemLALR1 = new ItemLALR1(Item);
                    if (i == 0)
                        ItemLALR1.Lookaheads.Add(TerminalEpsilon.Instance);
                    KernelLALR1.AddItem(ItemLALR1);
                }
                p_KernelsToLR0.Add(KernelLALR1, SetLR0);
                p_LR0ToKernels.Add(SetLR0, KernelLALR1);
            }
        }

        /// <summary>
        /// Build the propagation table
        /// </summary>
        private void BuildPropagationTable()
        {
            foreach (ItemSetKernel KernelLALR1 in p_KernelsToLR0.Keys)
            {
                ItemSet SetLR0 = p_KernelsToLR0[KernelLALR1];
                // For each LALR(1) item in the kernel
                // Only the kernel needs to be examined as the other items will be discovered and treated
                // with the dummy closures
                foreach (ItemLALR1 ItemLALR1 in KernelLALR1.Items)
                {
                    // If ItemLALR1 is of the form [A -> alpha .]
                    // => The closure will only contain the item itself
                    // => Cannot be used to generate or propagate lookaheads
                    if (ItemLALR1.Action == ItemAction.Reduce)
                        continue;
                    // Item here is of the form [A -> alpha . beta]
                    // Create the corresponding dummy item : [A -> alpha . beta, dummy]
                    // This item is used to detect lookahead propagation
                    ItemLALR1 DummyItem = new ItemLALR1(ItemLALR1);
                    DummyItem.Lookaheads.Add(TerminalDummy.Instance);
                    ItemSetKernel DummyKernel = new ItemSetKernel();
                    DummyKernel.AddItem(DummyItem);
                    ItemSet DummySet = DummyKernel.GetClosure();
                    // For each item in the closure of the dummy item
                    foreach (ItemLALR1 Item in DummySet.Items)
                    {
                        // If the item action is a reduction
                        // => OnSymbol for this item will be created by the LALR(1) closure
                        // => Do nothing
                        if (Item.Action == ItemAction.Reduce)
                            continue;
                        // Get the child item in the child LALR(1) kernel
                        // SetLR0.Children[Item.NextSymbol] is the child LR(0) set by a Item.NextSymbol transition
                        // p_LR0ToKernels[SetLR0.Children[Item.NextSymbol]] is then the associated LALR(1) kernel
                        ItemLALR1 ChildLALR1 = (ItemLALR1)GetEquivalentInSet(p_LR0ToKernels[SetLR0.Children[Item.NextSymbol]], Item.GetChild());
                        // If the lookaheads of the item in the dummy set contains the dummy terminal
                        if (Item.Lookaheads.Contains(TerminalDummy.Instance))
                        {
                            // => Propagation from the parent item to the child
                            p_PropagOrigins.Add(ItemLALR1);
                            p_PropagTargets.Add(ChildLALR1);
                            Item.Lookaheads.Remove(TerminalDummy.Instance);
                        }
                        if (Item.Lookaheads.Count != 0)
                        {
                            // => Spontaneous generation of lookaheads
                            ChildLALR1.Lookaheads.AddRange(Item.Lookaheads);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the child in the given kernel equals to the given copy
        /// </summary>
        /// <param name="Kernel">The kernel to search in</param>
        /// <param name="Equivalent">The searched value</param>
        /// <returns>Returns the item if found, null otherwise</returns>
        private static Item GetEquivalentInSet(ItemSetKernel Kernel, Item Equivalent)
        {
            foreach (Item Potential in Kernel.Items)
                if (Potential.Equals_Base(Equivalent))
                    return Potential;
            return null;
        }

        /// <summary>
        /// Propagate lookaheads in LALR(1) items using the propagation table
        /// </summary>
        private void BuildPropagate()
        {
            // Propagation table is built
            // Do passes to propagate
            int CountPass = 1;
            int CountModif = 1;
            while (CountModif != 0)
            {
                CountModif = 0;
                for (int i = 0; i != p_PropagOrigins.Count; i++)
                {
                    CountModif -= p_PropagTargets[i].Lookaheads.Count;
                    p_PropagTargets[i].Lookaheads.AddRange(p_PropagOrigins[i].Lookaheads);
                    CountModif += p_PropagTargets[i].Lookaheads.Count;
                }
                CountPass++;
            }
        }

        /// <summary>
        /// Build the final LALR(1) sets and graph from the LALR(1) kernels
        /// </summary>
        private void BuildGraphLALR1()
        {
            // Build sets
            p_GraphLALR1 = new Graph();
            foreach (ItemSetKernel KernelLALR1 in p_KernelsToLR0.Keys)
                p_GraphLALR1.Add(KernelLALR1.GetClosure());
            // Link and build actions for each LALR(1) set
            for (int i = 0; i != p_GraphLALR1.Sets.Count; i++)
            {
                ItemSet SetLALR1 = p_GraphLALR1.Sets[i];
                ItemSet SetLR0 = p_GraphLR0.Sets[i];
                // Set ID
                SetLALR1.ID = i;
                // Link
                foreach (Symbol Symbol in SetLR0.Children.Keys)
                {
                    ItemSet ChildLALR1 = p_GraphLALR1.Sets[p_GraphLR0.Sets.IndexOf(SetLR0.Children[Symbol])];
                    SetLALR1.Children.Add(Symbol, ChildLALR1);
                }
                // Build
                SetLALR1.BuildReductions(new ItemSetReductionsLALR1());
            }
        }

        /// <summary>
        /// Get the final LALR(1) graph
        /// </summary>
        /// <returns>Returns the LALR(1) graph</returns>
        public Graph GetGraphLALR1()
        {
            BuildKernels();
            BuildPropagationTable();
            BuildPropagate();
            BuildGraphLALR1();
            return p_GraphLALR1;
        }
    }
}