﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:22
 * 
 */
using System.Collections.Generic;
using Hime.Kernel.Reporting;

namespace Hime.Parsers.ContextFree.LR
{
    class MethodRNGLR1 : ParserGenerator
    {
        public MethodRNGLR1(Reporter reporter) : base("RNGLR(1)", reporter)
		{
		}

        protected override void OnBeginState(State state)
        {
            foreach (Conflict conflict in state.Conflicts)
                conflict.IsError = false;
        }
		
		protected override Graph BuildGraph (CFGrammar grammar)
		{
			return ConstructGraph(grammar);
		}
		
		protected override ParserData BuildParserData (CFGrammar grammar)
		{
			return new ParserDataRNGLR1(this.reporter, grammar, this.graph);
		}
		
		// TODO: try to remove static methods
        public static Graph ConstructGraph(CFGrammar grammar)
        {
            Graph GraphLR1 = MethodLR1.ConstructGraph(grammar);
            foreach (State Set in GraphLR1.States)
                Set.BuildReductions(new StateReductionsRNGLR1());
            return GraphLR1;
        }
    }
}
