﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:22
 * 
 */
namespace Hime.Kernel.Naming
{
    public abstract class NamingException : System.Exception
    {
        public NamingException() : base() { }
        public NamingException(string message) : base(message) { }
        public NamingException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}