using GridDomain.CQRS;
using Microsoft.AspNetCore.Mvc;

namespace Shop.Web.Controllers {
    public class OrderController : Controller
    {
        private readonly ICommandExecutor _commandBus;

        public OrderController(ICommandExecutor commandBus)
        {
            _commandBus = commandBus;
        }
    }
}