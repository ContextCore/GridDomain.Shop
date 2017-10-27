using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tools.Connector;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Shop.Node;
using Shop.Web.Identity;
using Swashbuckle.AspNetCore.Swagger;

namespace Shop.Web
{
    public class Startup
    {
        private ShopWebConfig Config { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Environment = env;
            Config = new ShopWebConfig();
            configuration.Bind(Config);
        }

        public IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection s)
        {
            s.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Shop API", Version = "v1" });
                c.AddSecurityDefinition("JWT",new OAuth2Scheme());
            });
            s.AddIdentity<AppUser, IdentityRole>
                    (o =>
                    {
                        // configure identity options
                        o.Password.RequireDigit = false;
                        o.Password.RequireLowercase = false;
                        o.Password.RequireUppercase = false;
                        o.Password.RequireNonAlphanumeric = false;
                        o.Password.RequiredLength = 6;
                    })
                    .AddEntityFrameworkStores<ShopIdentityDbContext>()
                    .AddDefaultTokenProviders();

            s.AddAutoMapper();
            // api user claim policy
            s.AddAuthorization(options =>
                                      {
                                          options.AddPolicy(Constants.Strings.AccessPolicy.ApiUser, policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
                                      });

          

            s.Configure<Identity.JwtIssuerOptions>(options =>
                                                 {
                                                     options.Issuer = Config.JwtIssuerOptions.Issuer;
                                                     options.Audience = Config.JwtIssuerOptions.Audience;
                                                     options.SigningCredentials = new SigningCredentials(CompositionRoot.SigningKey, SecurityAlgorithms.HmacSha256);
                                                 });


            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Config.JwtIssuerOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = Config.JwtIssuerOptions.Issuer,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = CompositionRoot.SigningKey,

                RequireExpirationTime = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            s.AddAuthentication(options =>
                                {
                                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                })
             .AddJwtBearer(o =>
                           {
                               o.TokenValidationParameters = tokenValidationParameters;
                               o.Events = new JwtBearerEvents()
                                          {
                                              OnAuthenticationFailed = c =>
                                                                       {
                                                                           c.NoResult();

                                                                           c.Response.StatusCode = 500;
                                                                           c.Response.ContentType = "text/plain";
                                                                           if(Environment.IsDevelopment())
                                                                           {
                                                                               // Debug only, in production do not share exceptions with the remote host.
                                                                               return c.Response.WriteAsync(c.Exception.ToString());
                                                                           }
                                                                           return c.Response.WriteAsync("An error occurred processing your authentication.");
                                                                       }
                                          };
                           });

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
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop API V1"));

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
            var node = Config.NodeOptions.Remote ? ConnectToNode() : CreateNode();
            CompositionRoot.Configure(builder, Config, node);
        }

        private IGridDomainNode CreateNode()
        {
            var node = new ShopNode(Config.NodeOptions.Name,
                                    Config.NodeOptions.Host,
                                    Config.NodeOptions.Port,
                                    Config.ConnectionStrings.ShopWrite,
                                    Config.ConnectionStrings.ShopRead);
            node.Start();
            return node.DomainNode;
        }

        private IGridDomainNode ConnectToNode()
        {
            var address = new NodeAddress()
                          {
                              Host = Config.NodeOptions.Host,
                              PortNumber = Config.NodeOptions.Port
                          };
            var connector = new GridNodeConnector(new NodeConfiguration(Config.NodeOptions.Name, address));
            Log.Information("started connect to griddomain node at {@address}", address);
            connector.Connect().Wait(TimeSpan.FromSeconds(10));
            if(!connector.IsConnected)
                throw new CannotConnectToNodeException();
            Log.Information("Connected to griddomain node at {@address}", address);
            return connector;
        }



    }
}
