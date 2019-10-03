namespace Quras.SmartContract.Framework.Services.Module
{
    public class Transaction : IScriptContainer
    {
        public extern byte[] Hash
        {
            [Syscall("Module.Transaction.GetHash")]
            get;
        }

        public extern byte Type
        {
            [Syscall("Module.Transaction.GetType")]
            get;
        }

        [Syscall("Module.Transaction.GetAttributes")]
        public extern TransactionAttribute[] GetAttributes();

        [Syscall("Module.Transaction.GetInputs")]
        public extern TransactionInput[] GetInputs();

        [Syscall("Module.Transaction.GetOutputs")]
        public extern TransactionOutput[] GetOutputs();

        [Syscall("Module.Transaction.GetReferences")]
        public extern TransactionOutput[] GetReferences();

        [Syscall("Module.Transaction.GetUnspentCoins")]
        public extern TransactionOutput[] GetUnspentCoins();
    }
}
