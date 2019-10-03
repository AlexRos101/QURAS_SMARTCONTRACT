namespace Quras.SmartContract.Framework.Services.Module
{
    public class TransactionInput : IApiInterface
    {
        public extern byte[] PrevHash
        {
            [Syscall("Module.Input.GetHash")]
            get;
        }

        public extern ushort PrevIndex
        {
            [Syscall("Module.Input.GetIndex")]
            get;
        }
    }
}
