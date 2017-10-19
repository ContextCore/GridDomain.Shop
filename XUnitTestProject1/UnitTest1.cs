using System;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Node;
using Xunit;

namespace XUnitTestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var str = ShopNode.DefaultNodeConfiguration;
            var aggregate = new User(Guid.NewGuid(), "hahahah",Guid.NewGuid());
        }
    }
}
