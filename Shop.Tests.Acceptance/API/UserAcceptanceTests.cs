using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shop.Web.Controllers;
using Shop.Web.Controllers.Domain;
using Should;
using Xunit;
using Xunit.Abstractions;

namespace Shop.Tests.Acceptance.API {

    [Collection(Constants.Collections.ShopAcceptance)]
    public class UserAcceptanceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ShopTestContext _shopTestContext;

        public UserAcceptanceTests(ITestOutputHelper output, ShopTestContext context)
        {
            _shopTestContext = context;
            _testOutputHelper = output;
        }

        [Fact]
        public async Task User_can_be_created()
        {
            var data = new CreateUserViewModel(){Login="testUser"};
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _shopTestContext.Client.PostAsync(Routes.Api.User.Create, content);

            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }
    }
}