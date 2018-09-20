using System;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using PeterKottas.DotNetCore.WindowsService;

namespace TFSLabelTagNotification
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log.txt");
            ServiceRunner<TFSLabelCheck>.Run(config =>
            {
                config.SetName("TFSNotificationService");
                config.SetDisplayName("Team Foundation Notification Service");

                var name = config.GetDefaultName();
                config.Service(serviceConfig =>
                {
                    serviceConfig.ServiceFactory((extraArguments, controller) =>
                    {
                        return new TFSLabelCheck(controller);
                    });

                    serviceConfig.OnStart((service, extraParams) =>
                    {
                        Console.WriteLine("Service {0} started", name);
                        service.Start();
                    });

                    serviceConfig.OnStop(service =>
                    {
                        Console.WriteLine("Service {0} stopped", name);
                        service.Stop();
                    });

                    serviceConfig.OnError(e =>
                    {
                        File.AppendAllText(fileName, $"Exception: {e.ToString()}\n");
                        Console.WriteLine("Service {0} errored with exception : {1}", name, e.Message);
                    });
                });
            });
        }
    }
}
