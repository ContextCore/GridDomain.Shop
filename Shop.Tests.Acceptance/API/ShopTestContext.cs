using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Shop.Web;

namespace Shop.Tests.Acceptance.API {
    public class ShopTestContext : IDisposable
    {
        private TestServer _server;
        private readonly IsolatedShopNode _isolatedShopNode;

        public HttpClient Client { get; private set; }

        public ShopTestContext()
        {
            SetUpClient();
            _isolatedShopNode = new IsolatedShopNode();
            _isolatedShopNode.Start();
        }

//        "ConnectionStrings": {
//            "ShopIdentity": "Server = (local); Database = ShopIdentity; Integrated Security = true; MultipleActiveResultSets = True",
//            "ShopSequences": "Server = (local); Database = ShopWrite; Integrated Security = true; MultipleActiveResultSets = True"
//        },
//    "JwtIssuerOptions": {
//    "Issuer": "griddomain_shop_example",
//    "Audience": "https://localhost:44342/"
//}
//}

    private IDictionary<string,string> Configuration = new Dictionary<string,string>
                                                           {
                                                                {"ConnectionStrings:ShopIdentity","Server = (local); Database = ShopIdentity; Integrated Security = true; MultipleActiveResultSets = True" },
                                                                {"ConnectionStrings:ShopSequences","Server = (local); Database = ShopIdentity; Integrated Security = true; MultipleActiveResultSets = True" },
                                                                {"JwtIssuerOptions:Issuer","Server = (local); Database = ShopIdentity; Integrated Security = true; MultipleActiveResultSets = True" },
                                                                {"JwtIssuerOptions:Audience","Server = (local); Database = ShopIdentity; Integrated Security = true; MultipleActiveResultSets = True" }
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
            _isolatedShopNode?.Dispose();
        }
    }
}