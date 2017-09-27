using GridDomain.CQRS;
using Microsoft.AspNetCore.Mvc;

namespace Shop.Web.Controllers.Domain {
    public class OrderController : Controller
    {
        private readonly ICommandExecutor _commandBus;

        public OrderController(ICommandExecutor commandBus)
        {
            _commandBus = commandBus;
        }
    }
}