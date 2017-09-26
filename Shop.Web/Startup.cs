using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Tools.Connector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shop.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var node = ConnectToNode();
            builder.RegisterInstance(node).As<IGridDomainNode>();
            builder.RegisterInstance(node).As<ICommandExecutor>();
        }

        
        private IGridDomainNode ConnectToNode()
        {
            var connector = new GridNodeConnector(new ShopNodeAddress());
            connector.Connect().Wait();
            return connector;
        }
    }
    class ShopNodeAddress : IAkkaNetworkAddress
    {
        public string SystemName { get; } = "shop_console";
        public string Host { get; } = "localhost";
        public string PublicHost { get; } = "localhost";
        public int PortNumber { get; } = 5001;
        public bool EnforceIpVersion { get; } = true;
    }
}
