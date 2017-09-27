using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shop.Web.Identity {
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ShopIdentityDbContext>
    {
        public ShopIdentityDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ShopIdentityDbContext>();
            builder.UseSqlServer("Server = (local); Database = ShopIdentity; Integrated Security = true; MultipleActiveResultSets = True");
            return new ShopIdentityDbContext(builder.Options);
        }
    }
}