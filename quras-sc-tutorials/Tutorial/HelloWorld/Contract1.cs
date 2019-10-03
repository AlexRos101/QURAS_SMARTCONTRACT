using Quras.SmartContract.Framework;
using Quras.SmartContract.Framework.Services.Module;
using System;
using System.Numerics;

namespace HelloWorld
{
    public class Contract1 : SmartContract
    {
        private static string version = "1.0.5";

        public static bool Main(string operation, object[] args)
        {
            Runtime.Log(version);
            Storage.Put(Storage.CurrentContext, "Hello Function", "World");
            return true;
        }
    }
}
