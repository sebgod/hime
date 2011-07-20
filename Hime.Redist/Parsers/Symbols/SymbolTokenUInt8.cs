﻿using System.Collections.Generic;

namespace Hime.Redist.Parsers
{
    /// <summary>
    /// Represents an unsigned byte matched by a lexer
    /// </summary>
    public sealed class SymbolTokenUInt8 : SymbolToken
    {
        private byte value;
        /// <summary>
        /// Gets the data represented by this symbol
        /// </summary>
        public override object Value { get { return value; } }
        /// <summary>
        /// Get the binary data represented by this token
        /// </summary>
        public byte ValueUInt8 { get { return value; } }
        /// <summary>
        /// Initializes a new instance of the SymbolTokenUInt8 class
        /// </summary>
        /// <param name="ClassName">Token's class name</param>
        /// <param name="ClassSID">Token's class ID</param>
        /// <param name="Value">Token binary value</param>
        public SymbolTokenUInt8(string ClassName, ushort ClassSID, byte Value) : base(ClassSID, ClassName) { value = Value; }
    }
}