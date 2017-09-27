using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Shop.Web.Identity {
    public class ShopIdentityDbContext : IdentityDbContext
    {
        public ShopIdentityDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}