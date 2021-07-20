using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.Web.Extensions
{
    public interface IKeyVaultService
    {
        public string GetSecret(string secretName);
    }

    public class KeyVaultService : IKeyVaultService
    {
        private readonly string _vaultUri;

        public KeyVaultService(IConfiguration configuration)
        {
            _vaultUri = configuration.GetConnectionString("VaultUri");
        }

        public string GetSecret(string secretName)
        {
            SecretClientOptions options = new SecretClientOptions()
            {
                Retry =
                {
                    Delay= TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                 }
            };
            var client = new SecretClient(new Uri(_vaultUri), new DefaultAzureCredential(), options);

            KeyVaultSecret secret = client.GetSecret(secretName);

            return secret.Value;
        }
    }
}
