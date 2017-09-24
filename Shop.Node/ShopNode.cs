using GridDomain.Node;
using PeterKottas.DotNetCore.WindowsService.Interfaces;

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
            _gridDomainNode.Start().
                            Wait();
        }

        public void Stop()
        {
            _gridDomainNode.Stop().
                            Wait();
        }
    }
}