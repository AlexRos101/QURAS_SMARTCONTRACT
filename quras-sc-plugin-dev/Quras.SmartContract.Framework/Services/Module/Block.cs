namespace Quras.SmartContract.Framework.Services.Module
{
    public class Block : Header
    {
        [Syscall("Module.Block.GetTransactionCount")]
        public extern int GetTransactionCount();

        [Syscall("Module.Block.GetTransactions")]
        public extern Transaction[] GetTransactions();

        [Syscall("Module.Block.GetTransaction")]
        public extern Transaction GetTransaction(int index);
    }
}
