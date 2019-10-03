namespace Quras.SmartContract.Framework.Services.Module
{
    public class Header : IScriptContainer
    {
        public extern byte[] Hash
        {
            [Syscall("Module.Header.GetHash")]
            get;
        }

        public extern uint Version
        {
            [Syscall("Module.Header.GetVersion")]
            get;
        }

        public extern byte[] PrevHash
        {
            [Syscall("Module.Header.GetPrevHash")]
            get;
        }

        public extern byte[] MerkleRoot
        {
            [Syscall("Module.Header.GetMerkleRoot")]
            get;
        }

        public extern uint Timestamp
        {
            [Syscall("Module.Header.GetTimestamp")]
            get;
        }

        public extern uint Index
        {
            [Syscall("Module.Header.GetIndex")]
            get;
        }

        public extern ulong ConsensusData
        {
            [Syscall("Module.Header.GetConsensusData")]
            get;
        }

        public extern byte[] NextConsensus
        {
            [Syscall("Module.Header.GetNextConsensus")]
            get;
        }
    }
}
