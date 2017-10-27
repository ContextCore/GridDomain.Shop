using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.ReadModel.Context;

namespace Shop.ReadModel {
    public class UserProjectionsBuilder : IHandler<UserCreated>
    {
        private readonly Func<ShopDbContext> _contextFactory;

        public UserProjectionsBuilder(Func<ShopDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task Handle(UserCreated message, IMessageMetadata metadata = null)
        {
            using (var ctx = _contextFactory())
            {
                await ctx.Users.AddAsync(new User() {Created = message.CreatedTime, Id = message.SourceId, Login = message.Login});
                await ctx.SaveChangesAsync();
            }
        }
    }
}