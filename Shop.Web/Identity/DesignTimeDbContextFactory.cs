using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shop.Web.Identity {
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ShopIdentityDbContext>
    {
        public ShopIdentityDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ShopIdentityDbContext>();
            builder.UseSqlServer("Server = localhost;  Database = ShopIdentity; User = sa; Password=P@ssw0rd1;  MultipleActiveResultSets = True");
            return new ShopIdentityDbContext(builder.Options);
        }
    }
}