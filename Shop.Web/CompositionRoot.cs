using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tools.Connector;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Shop.Infrastructure;
using Shop.ReadModel.Context;
using Shop.ReadModel.Queries;
using Shop.Web.Identity;

namespace Shop.Web {
    public static class CompositionRoot
    {
        private const string SecretKey = "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH"; // todo: get this from somewhere secure
        public static readonly SymmetricSecurityKey SigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public static void Configure(ContainerBuilder builder, ShopWebConfig config, ICommandExecutor gridNode)
        {
            builder.RegisterInstance(gridNode).As<ICommandExecutor>();

            var sequenceProvider = new SqlSequenceProvider(config.ConnectionStrings.ShopSequences);
            sequenceProvider.Connect();

            builder.RegisterInstance(sequenceProvider).As<ISequenceProvider>();

            var options = new DbContextOptionsBuilder<ShopIdentityDbContext>().UseSqlServer(config.ConnectionStrings.ShopIdentity).Options;
            builder.Register(c => new ShopIdentityDbContext(options)).InstancePerLifetimeScope();

            
            // Configure JwtIssuerOptions
            builder.Register(c => new Identity.JwtIssuerOptions()
                                                 {
                                                     Issuer = config.JwtIssuerOptions.Issuer,
                                                     Audience = config.JwtIssuerOptions.Audience,
                                                     SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256)
                                                 });

            builder.RegisterType<JwtFactory>().As<IJwtFactory>().ExternallyOwned();


            var queriesRoot = new QueriesCompositionRoot(config.ConnectionStrings.ShopRead);
            queriesRoot.Configure(builder);
        }
    }

    internal class CannotConnectToNodeException : Exception { }

    //TODO:place in readmodel or find better place
    public class QueriesCompositionRoot
    {
        private readonly string _readDbConnectionString;

        public QueriesCompositionRoot(string readDbConnectionString)
        {
            _readDbConnectionString = readDbConnectionString;
        }

        public void Configure(ContainerBuilder builder)
        {
            var options = new DbContextOptionsBuilder<ShopDbContext>().UseSqlServer(_readDbConnectionString).Options;
            builder.Register<ShopDbContext>(c => new ShopDbContext(options))
                   .AsSelf()
                   .InstancePerLifetimeScope();

            builder.RegisterType<UserInfoQuery>()
                   .As<IUserInfoQuery>()
                   .InstancePerLifetimeScope();
        }

    }
}