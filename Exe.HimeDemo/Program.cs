﻿/*
 * @author Charles Hymans
 * */

using Hime.Demo.Tasks;
using Hime.CentralDogma;

namespace Hime.Demo
{
    public class Program
    {
        static void Main()
        {
            //IExecutable executable = new Compile("Languages\\FileCentralDogma.gram", "FileCentralDogma", "Hime.Demo.Generated", ParsingMethod.LALR1);
            IExecutable executable = new ParseGrammar();
            executable.Execute();
        }
    }
}
