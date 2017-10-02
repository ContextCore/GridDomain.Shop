using System;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Serilog;
using Shop.Composition;

namespace Shop.Node {
    public class ShopNode : IMicroService
    {
        private GridDomainNode _gridDomainNode;

        public void Start()
        {
            _gridDomainNode = CreateGridNode();
            _gridDomainNode.Start().Wait();
        }

        public void Stop()
        {
            _gridDomainNode.Stop().Wait();
        }

        private static GridDomainNode CreateGridNode()
        {
            var config = new AkkaConfiguration(new ShopNodeNetworkConfig(), new ShopNodeDbConfig());
            //Allow parameterised configuration here.
            var settings = new NodeSettings(() => config.CreateSystem()) { Log = Log.Logger };
            settings.Add(new ShopDomainConfiguration());
            Log.Information($"Created shop node at {config.Network.Host} on port {config.Network.PortNumber}");
            return new GridDomainNode(settings);
        }
    }
}