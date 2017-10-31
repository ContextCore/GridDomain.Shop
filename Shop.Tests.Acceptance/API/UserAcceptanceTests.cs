using System;
using System.Threading.Tasks;
using Shop.ReadModel.Context;
using Shop.Web.Controllers;
using Shop.Web.Controllers.Domain;
using Shop.Web.Identity.ViewModels;
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
        public async Task User_can_be_created_and_it_can_be_fetched()
        {
            var data = new RegistrationViewModel(){Email="testUser@test.com", Password="testPass1;", FirstName = "testFirstName",LastName = "testLastName"};
            var userCreated = await _shopTestContext.Client.Post<UserCreatedViewModel>(Routes.Api.User.Controller + Routes.Api.User.Create, data);

            userCreated.UserId.ShouldNotEqual(Guid.Empty);
            userCreated.AccountId.ShouldNotEqual(Guid.Empty);
            userCreated.AccountNumber.ShouldBeGreaterThan(0);

            var userReadModel = await _shopTestContext.Client.Get<UserViewModel>(Routes.Api.User.Controller + userCreated.UserId);
           
            userReadModel.Login.ShouldEqual(data.Email);
        }
    }
}