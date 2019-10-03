namespace Quras.SmartContract.Framework.Services.Module
{
    public static class Runtime
    {
        public static extern TriggerType Trigger
        {
            [Syscall("Module.Runtime.GetTrigger")]
            get;
        }

        public static extern uint Time
        {
            [Syscall("Module.Runtime.GetTime")]
            get;
        }

        [Syscall("Module.Runtime.CheckWitness")]
        public static extern bool CheckWitness(byte[] hashOrPubkey);

        [Syscall("Module.Runtime.Notify")]
        public static extern void Notify(params object[] state);

        [Syscall("Module.Runtime.Log")]
        public static extern void Log(string message);
    }
}
