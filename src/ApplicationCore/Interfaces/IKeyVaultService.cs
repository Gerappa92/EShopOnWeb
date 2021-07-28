namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IKeyVaultService
    {
        public string GetSecret(string secretName);
    }
}
