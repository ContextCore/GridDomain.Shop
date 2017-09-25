using GridDomain.Node.Configuration.Akka;

namespace Shop.Node {
    class ShopNodeNetworkConfig : IAkkaNetworkAddress
    {
        public string SystemName { get; } = "ShopNode";
        public string Host { get; } = "localhost";
        public string PublicHost { get; } = "localhost";
        public int PortNumber { get; } = 5001;
        public bool EnforceIpVersion { get; } = true;
    }
}