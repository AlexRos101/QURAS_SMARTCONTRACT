using Quras.SmartContract.Framework;
using Quras.SmartContract.Framework.Services.Module;
using System;
using System.Numerics;

namespace $safeprojectname$
{
	public class Contract1 : SmartContract
    {
        public static bool Main(string operation, object[] args)
        {
            Storage.Put(Storage.CurrentContext, "Hello", "World");
            return true;
        }
    }
}
