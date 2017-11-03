using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using GridDomain.Tests.Common;
using GridDomain.Tools.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Shop.Node;
using Shop.ReadModel.Context;
using Shop.Web;
using Shop.Web.Identity;
using Program = Shop.Web.Program;

namespace Shop.Tests.Acceptance.API {
    public class ShopTestContext : IDisposable
    {
        private TestServer _server;
        public HttpClient Client { get; private set; }


        public ShopTestContext()
        {
           var cfg = SetUpClient();
            var options = new DbContextOptionsBuilder<ShopDbContext>()
                .UseSqlServer(cfg.ConnectionStrings.ShopRead).Options;
            using (var c = new ShopDbContext(options))
            {
                c.Database.EnsureDeleted();
                c.Database.EnsureCreated();
            }
            var identityOptions = new DbContextOptionsBuilder<ShopIdentityDbContext>()
                .UseSqlServer(cfg.ConnectionStrings.ShopIdentity).Options;

            using (var c = new ShopIdentityDbContext(identityOptions))
            {
                c.Database.EnsureDeleted();
                c.Database.EnsureCreated();
            }

            TestDbTools.Truncate(cfg.ConnectionStrings.ShopWrite, "Snapshots", "Journal", "Metadata").Wait();
        }

    private IDictionary<string,string> Configuration { get; }= new Dictionary<string,string>
    {
         {"ConnectionStrings:ShopIdentity","Server = (local); Database = ShopIdentity; Integrated Security = true; MultipleActiveResultSets = True" },
         {"ConnectionStrings:ShopSequences","Server = (local); Database = ShopIdentity; Integrated Security = true; MultipleActiveResultSets = True" },
         {"ConnectionStrings:ShopWrite","Server = (local); Database = ShopWrite; Integrated Security = true; MultipleActiveResultSets = True" },
         {"ConnectionStrings:ShopRead","Server = (local); Database = ShopRead; Integrated Security = true; MultipleActiveResultSets = True" },
         {"JwtIssuerOptions:Issuer","Server = (local); Database = ShopIdentity; Integrated Security = true; MultipleActiveResultSets = True" },
         {"JwtIssuerOptions:Audience","Server = (local); Database = ShopIdentity; Integrated Security = true; MultipleActiveResultSets = True" },
         {"NodeOptions:Remote","false" },
         {"NodeOptions:Port","5003" },
         {"NodeOptions:Host","localhost" },
         {"NodeOptions:Name","ShopEmbendedNode" }
    };

        private ShopWebConfig SetUpClient()
        {
            IConfigurationBuilder cfgBuilder=null; 
            var hostBuilder = new WebHostBuilder().ConfigureAppConfiguration(b => cfgBuilder = b.AddInMemoryCollection(Configuration));
            _server = new TestServer(Program.Configure(hostBuilder));
            Client = _server.CreateClient();
            var cfg = new ShopWebConfig();
            cfgBuilder.Build().Bind(cfg);
            return cfg;
        }

        public void Dispose()
        {
            _server?.Dispose();
            Client?.Dispose();
        }
    }
}