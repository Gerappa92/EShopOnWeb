using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.eShopWeb.Infrastructure.Services
{

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
