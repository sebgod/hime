﻿/*
 * Author: Charles Hymans
 * Date: 06/08/2011
 * Time: 22:30
 * 
 */

namespace Hime.Parsers
{
	// TODO: try to remove enums (if possible, not always possible)
    public enum EParsingMethod : byte
    {
        LR0 = 1,
        LR1 = 2,
        LALR1 = 3,
        RNGLR1 = 4,
        RNGLALR1 = 0,
        LRStar = 5,
        LRAutomata = 6
    }
}
