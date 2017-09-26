using GridDomain.Node.Configuration.Akka;

namespace Shop.Web {
    class ShopNodeAddress : IAkkaNetworkAddress
    {
        public string SystemName { get; } = "shop_console";
        public string Host { get; } = "localhost";
        public string PublicHost { get; } = "localhost";
        public int PortNumber { get; } = 5002;
        public bool EnforceIpVersion { get; } = true;
    }
}