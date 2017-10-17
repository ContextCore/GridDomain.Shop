
using GridDomain.Node.Persistence.Sql;

namespace Shop.Node {
    class ShopNodeDbConfig : DefaultNodeDbConfiguration
    {
        public ShopNodeDbConfig() : base("Server = (local); Database = ShopWrite; Integrated Security = true; MultipleActiveResultSets = True")
        {

        }
    }
}