﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:22
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using Hime.Utils.Reporting;

namespace Hime.Parsers
{
    interface LexerData
    {
        IList<Terminal> Expected { get; }
        void ExportCode(StreamWriter codeStream, string className, AccessModifier modifier);
        void ExportData(BinaryWriter dataStream);
    }
}