﻿using Hime.Redist.AST;

namespace Hime.CentralDogma.Grammars.ContextFree
{
    class CFPlugin : CompilerPlugin
    {
        public GrammarLoader GetLoader(ASTNode node, Reporting.Reporter log)
        {
            return new CFGrammarLoader(node, log);
        }
    }
}
