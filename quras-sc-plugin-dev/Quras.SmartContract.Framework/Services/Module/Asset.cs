namespace Quras.SmartContract.Framework.Services.Module
{
    public class Asset
    {
        public extern byte[] AssetId
        {
            [Syscall("Module.Asset.GetAssetId")]
            get;
        }

        public extern byte AssetType
        {
            [Syscall("Module.Asset.GetAssetType")]
            get;
        }

        public extern long Amount
        {
            [Syscall("Module.Asset.GetAmount")]
            get;
        }

        public extern long Available
        {
            [Syscall("Module.Asset.GetAvailable")]
            get;
        }

        public extern byte Precision
        {
            [Syscall("Module.Asset.GetPrecision")]
            get;
        }

        public extern byte[] Owner
        {
            [Syscall("Module.Asset.GetOwner")]
            get;
        }

        public extern byte[] Admin
        {
            [Syscall("Module.Asset.GetAdmin")]
            get;
        }

        public extern byte[] Issuer
        {
            [Syscall("Module.Asset.GetIssuer")]
            get;
        }

        [Syscall("Module.Asset.Create")]
        public static extern Asset Create(byte asset_type, string name, long amount, byte precision, byte[] owner, byte[] admin, byte[] issuer);

        [Syscall("Module.Asset.Renew")]
        public extern uint Renew(byte years);
    }
}
