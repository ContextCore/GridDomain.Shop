using GridDomain.CQRS;
using Microsoft.AspNetCore.Mvc;

namespace Shop.Web.Controllers.Domain {
    public class SkuController : Controller
    {
        private readonly ICommandExecutor _commandBus;

        public SkuController(ICommandExecutor commandBus)
        {
            _commandBus = commandBus;
        }
    }
}