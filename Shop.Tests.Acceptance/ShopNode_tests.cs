using Shop.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Tools.Connector;
using Shop.Domain.Aggregates.UserAggregate.Commands;
using Xunit;

namespace Shop.Tests.Acceptance
{
    public class ShopNode_tests
    {
        [Fact]
        public async Task ShopNode_can_execute_commands()
        {
            var node = new ShopNode();
            node.Start();


            var connector = new GridNodeConnector(new ShopNodeNetworkConfig());

            await connector.Connect();
            await connector.Execute(new CreateUserCommand(Guid.NewGuid(), "test_login", Guid.NewGuid()));
        }
    }
}
