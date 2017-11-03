using System;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.ReadModel;
using Shop.ReadModel.Context;

namespace Shop.Composition {
    class UserDomainConfiguration : IDomainConfiguration {
        private readonly Func<ShopDbContext> _dbContextFactory;

        public UserDomainConfiguration(Func<ShopDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.New(new UserCommandsHandler()));
            builder.RegisterHandler<UserCreated, UserProjectionsBuilder>(c => new UserProjectionsBuilder(_dbContextFactory))
                   .AsSync();
        }
    }
}