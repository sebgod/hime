﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:22
 * 
 */
namespace Hime.Parsers
{
    class TerminalNull : Terminal
    {
        private static TerminalNull instance;
        private static readonly object _lock = new object();
        private TerminalNull() : base(0, string.Empty, 0) { }

        public static TerminalNull Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                        instance = new TerminalNull();
                    return instance;
                }
            }
        }

        public override string ToString() { return string.Empty; }
    }
}