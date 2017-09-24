using System;
using GridDomain.Configuration;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using Shop.Composition;
using System.ServiceProcess;
using PeterKottas.DotNetCore.WindowsService;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;

namespace Shop.Node
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            ServiceRunner<ShopNode>.Run(config =>
                                              {
                                                  var name = config.GetDefaultName();
                                                  config.SetName("Shop node");
                                                  config.Service(serviceConfig =>
                                                                 {
                                                                     serviceConfig.ServiceFactory((extraArguments, c) =>
                                                                                                  {
                                                                                                      return new ShopNode(Create());
                                                                                                  });
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
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }


        public class ShopNode : IMicroService
        {
            private readonly GridDomainNode _gridDomainNode;

            public ShopNode(GridDomainNode gridDomainNode)
            {
                _gridDomainNode = gridDomainNode;
            }
            public void Start()
            {
                _gridDomainNode.Start().
                                Wait();
            }

            public void Stop()
            {
                _gridDomainNode.Stop().
                                Wait();
            }
        }

        class ShopNodeDbConfig : DefaultAkkaDbConfiguration
        {
            public ShopNodeDbConfig():base("Server = (local); Database = ShopWrite; Integrated Security = true; MultipleActiveResultSets = True")
            {
                
            }
        }

        class ShopNodeNetworkConfig : IAkkaNetworkAddress
        {
            public string SystemName { get; } = "ShopNode";
            public string Host { get; } = "127.0.0.1";
            public string PublicHost { get; } = "127.0.0.1";
            public int PortNumber { get; } = 10001;
            public bool EnforceIpVersion { get; } = true;
        }

        private static GridDomainNode Create()
        {
            var config = new AkkaConfiguration(new ShopNodeNetworkConfig(), new ShopNodeDbConfig());
            //Allow parameterised configuration here.
            var settings = new NodeSettings(()=>config.CreateSystem());
            settings.Add(new ShopDomainConfiguration());
            Console.WriteLine($"Created shop node at {config.Network.Host} on port {config.Network.PortNumber}");
            return new GridDomainNode(settings);
        }
    }
}
