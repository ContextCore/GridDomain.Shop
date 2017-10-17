using System;
using System.Runtime.CompilerServices;
using PeterKottas.DotNetCore.WindowsService;
using Serilog;
[assembly:InternalsVisibleTo("Shop.Tests.Acceptance")]
namespace Shop.Node
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            Log.Logger = new ShopLogConfiguration().CreateLogger();
            ServiceRunner<ShopNode>.Run(config =>
                                              {
                                                  var name = config.GetDefaultName();
                                                  config.SetName("Shop node");
                                                  config.Service(serviceConfig =>
                                                                 {
                                                                     serviceConfig.ServiceFactory((extraArguments, c) => ShopNode.CreateDefault());
                                                                     serviceConfig.OnStart((service, extraArguments) =>
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
                                                                                               Console.WriteLine("Service {0} errored with exception : {1}", name, e.Message);
                                                                                           });
                                                                 });
                                              });

            Console.WriteLine("Press any key and enter to exit");
            Console.ReadLine();
        }
    }
}
