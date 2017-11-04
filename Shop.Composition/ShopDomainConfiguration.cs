using System;
using Autofac;
using GridDomain.Configuration;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Domain.DomainServices.PriceCalculator;
using Shop.Infrastructure;
using Shop.ReadModel.Context;
using Shop.ReadModel.DomanServices;

namespace Shop.Composition {
    public class ShopDomainConfiguration : IDomainConfiguration
    {
        private readonly DbContextOptions<ShopDbContext> _readModelContextOptions;
        private readonly string _dbConnectionString;
        private readonly IContainer _container;

        public ShopDomainConfiguration(string dbConnectionString) : this(
            new DbContextOptionsBuilder<ShopDbContext>().UseSqlServer(dbConnectionString).
                                                         Options)
        {
            _dbConnectionString = dbConnectionString;
        }
        private ShopDomainConfiguration(DbContextOptions<ShopDbContext> readModelContextOptions)
        {
            _readModelContextOptions = readModelContextOptions;
            var container = new ContainerBuilder();
            Compose(container);
            _container = container.Build();
        }

        private void Compose(ContainerBuilder container)
        {
            container.RegisterInstance<Func<ShopDbContext>>(() => new ShopDbContext(_readModelContextOptions));

            container.RegisterType<SkuPriceQuery>()
                     .As<ISkuPriceQuery>()
                     .ExternallyOwned();

            container.RegisterType<SqlPriceCalculator>().As<IPriceCalculator>().SingleInstance();
            container.RegisterInstance<ISequenceProvider>(new SqlSequenceProvider(_dbConnectionString));
            container.RegisterInstance<ILogger>(Log.Logger);

            container.RegisterType<BuyNowProcessDomainConfiguration>();
            container.RegisterType<SkuDomainConfiguration>();
            container.RegisterType<OrderDomainConfiguration>();
            container.RegisterType<UserDomainConfiguration>();
        }
      
        public void Register(IDomainBuilder builder)
        {
            builder.Register(_container.Resolve<BuyNowProcessDomainConfiguration>());
            builder.Register(_container.Resolve<SkuDomainConfiguration>());
            builder.Register(_container.Resolve<OrderDomainConfiguration>());
            builder.Register(_container.Resolve<UserDomainConfiguration>());

            builder.Register(new SkuStockDomainConfiguration());
            builder.Register(new AccountDomainConfiguration());
        }
    }
}