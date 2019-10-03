namespace Quras.SmartContract.Framework.Services.Module
{
    public class Iterator<TKey, TValue>
    {
        [Syscall("Module.Iterator.Next")]
        public extern bool Next();

        public extern TKey Key
        {
            [Syscall("Module.Iterator.Key")]
            get;
        }

        public extern TValue Value
        {
            [Syscall("Module.Iterator.Value")]
            get;
        }
    }
}
