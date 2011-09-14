﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:22
 * 
 */
namespace Hime.Kernel.Resources
{
    [System.Serializable]
    class CompilerException : System.Exception
    {
        public CompilerException() : base() { }
        public CompilerException(string message) : base(message) { }
        public CompilerException(string message, System.Exception inner) : base(message, inner) { }
        protected CompilerException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}