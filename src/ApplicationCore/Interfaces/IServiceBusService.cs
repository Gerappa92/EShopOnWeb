using Azure.Messaging.ServiceBus;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IServiceBusService
    {
        Task SendMessage<T>(string queueName, T messageObject) where T : class;
        ServiceBusProcessor GetProcessor(string queueName, ServiceBusProcessorOptions options);
    }
}
