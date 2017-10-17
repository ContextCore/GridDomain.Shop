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
using Shop.Web.Identity;

namespace Shop.Web {
    public static class CompositionRoot
    {
        private const string SecretKey = "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH"; // todo: get this from somewhere secure
        public static readonly SymmetricSecurityKey SigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public static void Configure(ContainerBuilder builder, IConfiguration configuration)
        {
            var node = ConnectToNode();
            builder.RegisterInstance(node).As<ICommandExecutor>();

           var sequenceProvider = new SqlSequenceProvider("Server = (local); Database = ShopRead; Integrated Security = true; MultipleActiveResultSets = True");
            sequenceProvider.Connect();

            builder.RegisterInstance(sequenceProvider).As<ISequenceProvider>();

            var options = new DbContextOptionsBuilder<ShopIdentityDbContext>().UseSqlServer(configuration.GetConnectionString("ShopIdentity")).Options;
            builder.Register(c => new ShopIdentityDbContext(options)).InstancePerLifetimeScope();


            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            builder.Register(c => new JwtIssuerOptions()
                                                 {
                                                     Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
                                                     Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                                                     SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256)
                                                 });

            builder.RegisterType<JwtFactory>().As<IJwtFactory>().ExternallyOwned();
        }

        private static IGridDomainNode ConnectToNode()
        {
            var address = new ShopNodeAddress();
            var connector = new GridNodeConnector(new NodeConfiguration("ShopNode", new ShopNodeAddress()));
            Log.Information("started connect to griddomain node at {@address}", address);
            connector.Connect()
                     .ContinueWith(t => Log.Information("Connected to griddomain node at {@address}", address),
                                        TaskContinuationOptions.OnlyOnRanToCompletion);
            return connector;
        }
    }
}