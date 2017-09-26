using GridDomain.CQRS;
using Microsoft.AspNetCore.Mvc;

namespace Shop.Web.Controllers {
    public class UserController : Controller
    {
        private readonly ICommandExecutor _commandBus;

        public UserController(ICommandExecutor commandBus)
        {
            _commandBus = commandBus;
        }
    }
}