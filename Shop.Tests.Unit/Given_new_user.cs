using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit
{
    public class Given_new_user
    {
        [Fact]
        public void It_should_emit_user_created_event()
        {
            var user =new User(Guid.NewGuid(),"test_login",Guid.NewGuid());
            var e = ((IAggregate)user).GetEvent<UserCreated>();
            ((IAggregate)user).CommitAll();
            Assert.Equal(e.Account, user.Account);
            Assert.Equal(e.SourceId, user.Id);
            Assert.Equal(e.Login, user.Login);
        }
    }
}