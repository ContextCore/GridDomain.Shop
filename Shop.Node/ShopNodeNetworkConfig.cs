
using GridDomain.Node.Configuration;

namespace Shop.Node {
    class ShopNodeNetworkConfig : INodeNetworkAddress
    {
        public string Host { get; } = "localhost";
        public string PublicHost { get; } = "localhost";
        public int PortNumber { get; } = 5002;
        public bool EnforceIpVersion { get; } = true;
    }
}