using GridDomain.Configuration;
using Shop.Domain.Aggregates.UserAggregate;

namespace Shop.Composition {
    class UserDomainConfiguration : IDomainConfiguration {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.New(new UserCommandsHandler()));

        }
    }
}