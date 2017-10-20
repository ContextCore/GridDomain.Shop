using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using GridDomain.Tools.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Shop.Node;
using Shop.ReadModel.Context;
using Program = Shop.Web.Program;

namespace Shop.Tests.Acceptance.API {
    public class ShopTestContext : IDisposable
    {
        private TestServer _server;

        public HttpClient Client { get; private set; }


        public ShopTestContext()
        {
            SetUpClient();
        }

    private IDictionary<string,string> Configuration { get; }= new Dictionary<string,string>
    {
         {"ConnectionStrings:ShopIdentity","Server = (local); Database = ShopIdentity; Integrated Security = true; MultipleActiveResultSets = True" },
         {"ConnectionStrings:ShopSequences","Server = (local); Database = ShopIdentity; Integrated Security = true; MultipleActiveResultSets = True" },
         {"JwtIssuerOptions:Issuer","Server = (local); Database = ShopIdentity; Integrated Security = true; MultipleActiveResultSets = True" },
         {"JwtIssuerOptions:Audience","Server = (local); Database = ShopIdentity; Integrated Security = true; MultipleActiveResultSets = True" },
         {"NodeOptions:Remote","false" },
         {"NodeOptions:Port","5003" },
         {"NodeOptions:Host","localhost" },
         {"NodeOptions:Name","ShopEmbendedNode" }
    };

        private void SetUpClient()
        {
            var hostBuilder = new WebHostBuilder().ConfigureAppConfiguration(b => b.AddInMemoryCollection(Configuration));
            _server = new TestServer(Program.Configure(hostBuilder));
            Client = _server.CreateClient();
        }

        public void Dispose()
        {
            _server?.Dispose();
            Client?.Dispose();
        }
    }
}