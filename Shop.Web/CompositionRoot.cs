using System;
using System.Configuration;
using Autofac;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Tools.Connector;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Shop.Web.Identity;

namespace Shop.Web {
    public static class CompositionRoot
    {
        public static void Configure(ContainerBuilder builder, IConfiguration configuration)
        {
            var node = ConnectToNode();
            builder.RegisterInstance(node).As<IGridDomainNode>();
            builder.RegisterInstance(node).As<ICommandExecutor>();

            var options = new DbContextOptionsBuilder<ShopIdentityDbContext>().UseSqlServer(configuration.GetConnectionString("ShopIdentity")).Options;
            builder.Register(c => new ShopIdentityDbContext(options)).InstancePerLifetimeScope();

          
            //AddDbContext<ApplicationDbContext>(options =>
            //                                            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
            //                                                                 b => b.MigrationsAssembly("DotNetGigs")));
        }

        private static IGridDomainNode ConnectToNode()
        {
            var address = new ShopNodeAddress();
            var connector = new GridNodeConnector(address, null, TimeSpan.FromMinutes(1));
            Log.Information("started connect to griddomain node at {@address}", address);

            //for exception propagation without AggregatException wrapper
            connector.Connect()
                     .ContinueWith(t => Log.Information("Connected to griddomain node at {@address}", address));
            return connector;
        }
    }
}