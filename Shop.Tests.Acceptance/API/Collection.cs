using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Shop.Tests.Acceptance.API
{
    [CollectionDefinition(Constants.Collections.ShopAcceptance)]
    public class Collection : ICollectionFixture<ShopTestContext>
    {

    }
}
