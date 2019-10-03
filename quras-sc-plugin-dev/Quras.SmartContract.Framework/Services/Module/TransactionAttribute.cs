namespace Quras.SmartContract.Framework.Services.Module
{
    public class TransactionAttribute : IApiInterface
    {
        public extern byte Usage
        {
            [Syscall("Module.Attribute.GetUsage")]
            get;
        }

        public extern byte[] Data
        {
            [Syscall("Module.Attribute.GetData")]
            get;
        }
    }
}
