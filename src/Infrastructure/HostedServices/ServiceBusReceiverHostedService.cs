using Azure.Messaging.ServiceBus;
using Flurl.Http;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Infrastructure.HostedServices
{
    public class ServiceBusReceiverHostedService : IHostedService, IDisposable
    {
        private IServiceBusService _serviceBusService;
        private IKeyVaultService _keyVaultService;
        ServiceBusProcessor _process;

        public ServiceBusReceiverHostedService(IServiceBusService serviceBusService, IKeyVaultService keyVaultService)
        {
            _serviceBusService = serviceBusService;
            _keyVaultService = keyVaultService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _process = _serviceBusService.GetProcessor("orders", new ServiceBusProcessorOptions()
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 2,
            });
            _process.ProcessMessageAsync += MessageHandler;
            _process.ProcessErrorAsync += ErrorHandler;
            _process.StartProcessingAsync();
            return Task.CompletedTask;
        }

        private async Task MessageHandler(ProcessMessageEventArgs arg)
        {
            string body = arg.Message.Body.ToString();
            await _keyVaultService.GetSecret("FunctionAddOrdersJson").PostJsonAsync(body);
            await arg.CompleteMessageAsync(arg.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            Debug.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _process.StopProcessingAsync();
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _process.StopProcessingAsync();
            _process.DisposeAsync();
        }
    }
}
