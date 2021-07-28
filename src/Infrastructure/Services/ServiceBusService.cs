using Azure.Messaging.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Infrastructure.Services
{
    public class ServiceBusService : IServiceBusService
    {
        private readonly string _serviceBusConnectionString;

        public ServiceBusService(IKeyVaultService keyVaultService)
        {
            _serviceBusConnectionString = keyVaultService.GetSecret("ServiceBusConnectionString");
        }

        public async Task SendMessage<T>(string queueName, T messageObject) where T : class
        {
            var client = CreateClient();
            ServiceBusSender sender = client.CreateSender(queueName);
            var messageJson = JsonExtensions.ToJson(messageObject);
            var message = new ServiceBusMessage(messageJson);
            await sender.SendMessageAsync(message);
        }

        public ServiceBusProcessor GetProcessor(string queueName, ServiceBusProcessorOptions options)
        {
            var client = CreateClient();
            return client.CreateProcessor(queueName, options);
        }

        private ServiceBusClient CreateClient()
        {
            var retryOptions = new ServiceBusRetryOptions()
            {
                MaxRetries = 3,
                Delay = TimeSpan.FromSeconds(5)
            };
            var options = new ServiceBusClientOptions()
            {
                RetryOptions = retryOptions,
            };
            return new ServiceBusClient(_serviceBusConnectionString, options);
        }
    }
}
