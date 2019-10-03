namespace Quras.SmartContract.Framework.Services.Module
{
    public class Account
    {
        public extern byte[] ScriptHash
        {
            [Syscall("Module.Account.GetScriptHash")]
            get;
        }

        public extern byte[][] Votes
        {
            [Syscall("Module.Account.GetVotes")]
            get;
        }

        [Syscall("Module.Account.GetBalance")]
        public extern long GetBalance(byte[] asset_id);
    }
}
