using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using System.Threading.Tasks;
using Flurl.Http;
using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.Infrastructure.Services
{
    public class AzureFunctionService : IAzureFunctionService
    {
        private readonly IKeyVaultService _keyVaultService;
        private readonly ILogger<AzureFunctionService> _logger;

        public AzureFunctionService(IKeyVaultService keyVaultService, ILogger<AzureFunctionService> logger)
        {
            _keyVaultService = keyVaultService;
            _logger = logger;
        }

        public async Task AddOrdersToCosmosDB(Order order)
        {
            try
            {
                await _keyVaultService.GetSecret("FunctionAddOrdersToCosmoDB").PostJsonAsync(order);
            }
            catch (Exception)
            {
                _logger.LogWarning("The orders were not added to the cosmos DB");
            }
        }
    }
}
