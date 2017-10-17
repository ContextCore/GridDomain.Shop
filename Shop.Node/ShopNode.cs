using System;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Serilog;
using Shop.Composition;

namespace Shop.Node
{
    public class ShopNode : IMicroService
    {
        private GridDomainNode _gridDomainNode;
        public static NodeConfiguration DefaultNodeConfiguration { get; } = new ShopNodeConfiguration();
        public static ISqlNodeDbConfiguration DefaultPersistenceConfiguration { get; } = new ShopNodeDbConfig();
        public static string DefaultReadDbConnectionString { get; } = "";

        public NodeConfiguration NodeConfiguration { get; } 
        public ISqlNodeDbConfiguration PersistenceConfiguration { get; }
        public string ReadDbConnectionString { get; }


        public class ShopNodeConfiguration : NodeConfiguration
        {
            public ShopNodeConfiguration() : base("ShopNode", new ShopNodeNetworkConfig())
            {

            }
        }

        public ShopNode(string name, string host, int port, string writeDbConnectionString, string readDbConnectionString)
            :this(new NodeConfiguration(name,new NodeNetworkAddress(host,port)), new DefaultNodeDbConfiguration(writeDbConnectionString), readDbConnectionString)
        {
        }

        public ShopNode(NodeConfiguration cfg, ISqlNodeDbConfiguration persistenceCfg, string readModelString)
        {
            NodeConfiguration = cfg;
            PersistenceConfiguration = persistenceCfg;
            ReadDbConnectionString = readModelString;
        }

        public static ShopNode CreateDefault()
        {
            return new ShopNode(DefaultNodeConfiguration, DefaultPersistenceConfiguration, DefaultReadDbConnectionString);
        }

        public void Start()
        {
            Log.Information($"Created shop node {NodeConfiguration.Name} at {NodeConfiguration.Address.Host} on port {NodeConfiguration.Address.PortNumber}");

            var actorSystemFactory = ActorSystemBuilder.New().Build(NodeConfiguration, new ShopNodeDbConfig());

            _gridDomainNode = new GridDomainNode(actorSystemFactory, new ShopDomainConfiguration(ReadDbConnectionString));
            _gridDomainNode.Start().Wait();
        }

        public void Stop()
        {
            _gridDomainNode.Stop().Wait();
        }
    }


}