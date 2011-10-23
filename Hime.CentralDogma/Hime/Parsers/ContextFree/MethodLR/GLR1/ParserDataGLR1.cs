﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:22
 * 
 */
using System;
using System.IO;
using System.Collections.Generic;
using Hime.Kernel.Reporting;

namespace Hime.Parsers.ContextFree.LR
{
    class ParserDataGLR1 : ParserDataLR
    {
        public ParserDataGLR1(Reporter reporter, CFGrammar gram, Graph graph) : base(reporter, gram, graph) { }

		internal protected override string GetBaseClassName 
		{
			// TODO: why is it a NotImplementedException here? Will it be implemented some day, or is it a problem in the class hierarchy?
			get { throw new NotImplementedException ();	}
		}

        public override void Export(StreamWriter stream, string className, AccessModifier modifier, string lexerClassName, IList<Terminal> expected, bool exportDebug)
        {
			// TODO: why is it a NotImplementedException here? Will it be implemented some day, or is it a problem in the class hierarchy?
            throw new NotImplementedException();
        }
		
		protected override void ExportRules (System.IO.StreamWriter stream)
		{
			// TODO: why is it a NotImplementedException here? Will it be implemented some day, or is it a problem in the class hierarchy?
			throw new NotImplementedException ();
		}
		
		protected override void ExportStates (System.IO.StreamWriter stream)
		{
			throw new NotImplementedException ();
		}
    }
}
