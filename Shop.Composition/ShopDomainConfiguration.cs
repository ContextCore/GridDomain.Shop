using System;
using Autofac;
using GridDomain.Configuration;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.DomainServices.PriceCalculator;
using Shop.Infrastructure;
using Shop.ReadModel.Context;
using Shop.ReadModel.DomanServices;

namespace Shop.Composition {

    public static class UnityEtensions
    {
        public static void RegisterType<TAbstr, TImpl>(this ContainerBuilder builder)
        {
            builder.RegisterType<TImpl>().
                    As<TAbstr>();
        }
    }
    public class ShopDomainConfiguration : IDomainConfiguration
    {
        private readonly DbContextOptions<ShopDbContext> _readModelContextOptions;
        private readonly IContainer _container;

        public ShopDomainConfiguration(DbContextOptions<ShopDbContext> readModelContextOptions = null)
        {
            _readModelContextOptions = readModelContextOptions
                                       ?? new DbContextOptionsBuilder<ShopDbContext>().UseSqlServer(
                                                                                          "Server = (local); Database = Shop; Integrated Security = true; MultipleActiveResultSets = True")
                                                                                      .Options;
            var container = new ContainerBuilder();
            Compose(container);
            _container = container.Build();
        }

        private void Compose(ContainerBuilder container)
        {
            container.RegisterInstance<Func<ShopDbContext>>(() => new ShopDbContext(_readModelContextOptions));
            container.RegisterType<ISkuPriceQuery, SkuPriceQuery>();
            container.RegisterType<IPriceCalculator, SqlPriceCalculator>();
            container.RegisterType<ISequenceProvider, SqlSequenceProvider>();
            container.RegisterType<BuyNowProcessDomainConfiguration>();
            container.RegisterType<AccountDomainConfiguration>();
            container.RegisterType<SkuDomainConfiguration>();
            container.RegisterType<OrderDomainConfiguration>();
            container.RegisterType<SkuStockDomainConfiguration>();
            container.RegisterType<UserDomainConfiguration>();
           
        }
      
        public void Register(IDomainBuilder builder)
        {
            builder.Register(_container.Resolve<BuyNowProcessDomainConfiguration>());
            builder.Register(_container.Resolve<AccountDomainConfiguration>());
            builder.Register(_container.Resolve<SkuDomainConfiguration>());
            builder.Register(_container.Resolve<OrderDomainConfiguration>());
            builder.Register(_container.Resolve<SkuStockDomainConfiguration>());
            builder.Register(_container.Resolve<UserDomainConfiguration>());
        }
    }
}