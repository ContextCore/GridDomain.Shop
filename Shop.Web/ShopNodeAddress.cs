using GridDomain.Node.Configuration;

namespace Shop.Web {
    class NodeAddress : INodeNetworkAddress
    {
        public string Host { get; set; }
        public string PublicHost => Host;
        public int PortNumber { get; set; }
        public bool EnforceIpVersion => true;
    }
}