namespace Quras.SmartContract.Framework.Services.Module
{
    public static class Blockchain
    {
        [Syscall("Module.Blockchain.GetHeight")]
        public static extern uint GetHeight();

        [Syscall("Module.Blockchain.GetHeader")]
        public static extern Header GetHeader(uint height);

        [Syscall("Module.Blockchain.GetHeader")]
        public static extern Header GetHeader(byte[] hash);

        [Syscall("Module.Blockchain.GetBlock")]
        public static extern Block GetBlock(uint height);

        [Syscall("Module.Blockchain.GetBlock")]
        public static extern Block GetBlock(byte[] hash);

        [Syscall("Module.Blockchain.GetTransaction")]
        public static extern Transaction GetTransaction(byte[] hash);

        [Syscall("Module.Blockchain.GetAccount")]
        public static extern Account GetAccount(byte[] script_hash);

        [Syscall("Module.Blockchain.GetValidators")]
        public static extern byte[][] GetValidators();

        [Syscall("Module.Blockchain.GetAsset")]
        public static extern Asset GetAsset(byte[] asset_id);

        [Syscall("Module.Blockchain.GetContract")]
        public static extern Contract GetContract(byte[] script_hash);
    }
}
