using GridDomain.Node.Configuration.Akka;

namespace Shop.Node {
    class ShopNodeDbConfig : DefaultAkkaDbConfiguration
    {
        public ShopNodeDbConfig() : base("Server = (local); Database = ShopWrite; Integrated Security = true; MultipleActiveResultSets = True")
        {

        }
    }
}