﻿using System;

namespace Quras.Compiler.ASM
{
    public class CompilerException : Exception
    {
        public CompilerException(uint lineNumber, string message)
            : base($"ERROR: {message} in line {lineNumber}.")
        {
        }
    }
}
