using GridDomain.Node;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Serilog;

namespace Shop.Node {
    public class ShopNode : IMicroService
    {
        private readonly GridDomainNode _gridDomainNode;

        public ShopNode(GridDomainNode gridDomainNode)
        {
            _gridDomainNode = gridDomainNode;
        }
        public void Start()
        {
            _gridDomainNode.Start().Wait();
            //StartBase();
           // Timers.Start("heartbeat",1000,()=> Log.Information("heartbeat"));
        }

        public void Stop()
        {
            _gridDomainNode.Stop().Wait();
        }
    }
}