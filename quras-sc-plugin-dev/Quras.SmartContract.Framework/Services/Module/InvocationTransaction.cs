namespace Quras.SmartContract.Framework.Services.Module
{
    public class InvocationTransaction : Transaction
    {
        public extern byte[] Script
        {
            [Syscall("Module.InvocationTransaction.GetScript")]
            get;
        }
    }
}
