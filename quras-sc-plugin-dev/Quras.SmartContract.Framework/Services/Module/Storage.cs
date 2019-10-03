using System.Numerics;

namespace Quras.SmartContract.Framework.Services.Module
{
    public static class Storage
    {
        public static extern StorageContext CurrentContext
        {
            [Syscall("Module.Storage.GetContext")]
            get;
        }

        [Syscall("Module.Storage.Get")]
        public static extern byte[] Get(StorageContext context, byte[] key);

        [Syscall("Module.Storage.Get")]
        public static extern byte[] Get(StorageContext context, string key);

        [Syscall("Module.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, byte[] value);

        [Syscall("Module.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, BigInteger value);

        [Syscall("Module.Storage.Put")]
        public static extern void Put(StorageContext context, byte[] key, string value);

        [Syscall("Module.Storage.Put")]
        public static extern void Put(StorageContext context, string key, byte[] value);

        [Syscall("Module.Storage.Put")]
        public static extern void Put(StorageContext context, string key, BigInteger value);

        [Syscall("Module.Storage.Put")]
        public static extern void Put(StorageContext context, string key, string value);

        [Syscall("Module.Storage.Delete")]
        public static extern void Delete(StorageContext context, byte[] key);

        [Syscall("Module.Storage.Delete")]
        public static extern void Delete(StorageContext context, string key);

        [Syscall("Module.Storage.Find")]
        public static extern Iterator<byte[], byte[]> Find(StorageContext context, byte[] prefix);

        [Syscall("Module.Storage.Find")]
        public static extern Iterator<string, byte[]> Find(StorageContext context, string prefix);
    }
}
