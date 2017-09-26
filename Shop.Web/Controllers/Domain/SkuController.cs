using GridDomain.CQRS;
using Microsoft.AspNetCore.Mvc;

namespace Shop.Web.Controllers {
    public class SkuController : Controller
    {
        private readonly ICommandExecutor _commandBus;

        public SkuController(ICommandExecutor commandBus)
        {
            _commandBus = commandBus;
        }
    }
}