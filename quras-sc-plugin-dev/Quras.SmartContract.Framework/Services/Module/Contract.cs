namespace Quras.SmartContract.Framework.Services.Module
{
    public class Contract
    {
        public extern byte[] Script
        {
            [Syscall("Module.Contract.GetScript")]
            get;
        }
       
        public extern bool IsPayable
        {
            [Syscall("Module.Contract.IsPayable")]
            get;
        }

        public extern StorageContext StorageContext
        {
            [Syscall("Module.Contract.GetStorageContext")]
            get;
        }

        [Syscall("Module.Contract.Create")]
        public static extern Contract Create(byte[] script, byte[] parameter_list, byte return_type, bool need_storage, string name, string version, string author, string email, string description);

        [Syscall("Module.Contract.Migrate")]
        public static extern Contract Migrate(byte[] script, byte[] parameter_list, byte return_type, bool need_storage, string name, string version, string author, string email, string description);

        [Syscall("Module.Contract.Destroy")]
        public static extern void Destroy();
    }
}
