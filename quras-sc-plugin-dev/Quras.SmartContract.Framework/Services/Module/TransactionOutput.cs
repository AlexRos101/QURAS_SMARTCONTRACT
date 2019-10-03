namespace Quras.SmartContract.Framework.Services.Module
{
    public class TransactionOutput : IApiInterface
    {
        public extern byte[] AssetId
        {
            [Syscall("Module.Output.GetAssetId")]
            get;
        }

        public extern long Value
        {
            [Syscall("Module.Output.GetValue")]
            get;
        }

        public extern byte[] ScriptHash
        {
            [Syscall("Module.Output.GetScriptHash")]
            get;
        }
    }
}
