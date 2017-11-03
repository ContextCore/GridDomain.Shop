using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using Shop.Composition;
using Xunit;

namespace Shop.Tests.Unit
{
    public class ContainerTests
    {
        [Fact]
        public void AllRegistrationsMustBeResolvable()
        {
            var container = new ContainerBuilder();
            var cfg = new ShopDomainConfiguration();
            var domainBuilder = new DomainBuilder();
            domainBuilder.Register(cfg);
            domainBuilder.Configure(container);
            ResolveAll(container.Build());
        }
        //TODO: add named services resolution
        private void ResolveAll(IContainer container)
        {
            Console.WriteLine();
            var errors = new Dictionary<IServiceWithType, Exception>();
            foreach(var reg in container.ComponentRegistry.Registrations.SelectMany(x => x.Services)
                                        .OfType<IServiceWithType>()
                                        .Where(r => !r.ServiceType.Name.Contains("Actor")))
                try
                {
                    container.Resolve(reg.ServiceType);
                    Console.WriteLine($"resolved {reg.ServiceType}");
                }
                catch(Exception ex)
                {
                    errors[reg] = ex;
                }

            Console.WriteLine();
            if(!errors.Any())
                return;

            var builder = new StringBuilder();
            foreach(var error in errors.Take(5))
                builder.AppendLine($"Exception while resolving {error.Key.ServiceType} : {error.Value}");

            Assert.True(false, "Can not resolve registrations: \r\n " + builder);
        }
    }
}
