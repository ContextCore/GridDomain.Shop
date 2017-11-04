using System;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Serilog;
using Shop.Composition;

namespace Shop.Node
{
    public class ShopNode : IMicroService, IDisposable
    {
        public GridDomainNode DomainNode { get; private set; }
        private readonly ILogger _logger;
        public static NodeConfiguration DefaultNodeConfiguration { get; } = new ShopNodeConfiguration();
        public static ISqlNodeDbConfiguration DefaultPersistenceConfiguration { get; } = new ShopNodeDbConfig();
        public static string DefaultReadDbConnectionString { get; } = @"Server = localhost;  Database = ShopRead; User = sa; Password=P@ssw0rd1;  MultipleActiveResultSets = True";

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

        public ShopNode(NodeConfiguration cfg, ISqlNodeDbConfiguration persistenceCfg, string readModelString, ILogger logger = null)
        {
            _logger = logger;
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

            DomainNode = new GridDomainNode(actorSystemFactory, _logger ?? Log.Logger, new ShopDomainConfiguration(ReadDbConnectionString));
            DomainNode.Start().Wait();
        }

        public void Stop()
        {
            DomainNode.Stop().Wait();
        }

        public void Dispose()
        {
            DomainNode.Dispose();
        }
    }


}