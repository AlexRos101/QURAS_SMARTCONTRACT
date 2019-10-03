using System;

namespace Quras.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor)]
    public class NonemitAttribute : Attribute
    {
    }
}
