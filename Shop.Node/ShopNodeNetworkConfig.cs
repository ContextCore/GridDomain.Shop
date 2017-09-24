using GridDomain.Node.Configuration.Akka;

namespace Shop.Node {
    class ShopNodeNetworkConfig : IAkkaNetworkAddress
    {
        public string SystemName { get; } = "ShopNode";
        public string Host { get; } = "127.0.0.1";
        public string PublicHost { get; } = "127.0.0.1";
        public int PortNumber { get; } = 10001;
        public bool EnforceIpVersion { get; } = true;
    }
}