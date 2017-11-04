
using GridDomain.Node.Persistence.Sql;

namespace Shop.Node {
    class ShopNodeDbConfig : DefaultNodeDbConfiguration
    {
        public ShopNodeDbConfig() : base("Server = localhost;  Database = ShopWrite; User = sa; Password=P@ssw0rd1;  MultipleActiveResultSets = True")
        {

        }
    }
}