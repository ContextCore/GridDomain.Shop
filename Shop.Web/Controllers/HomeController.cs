using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Web.Identity;

namespace Shop.Web.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly ICommandExecutor _bus;

        public HomeController(ICommandExecutor commandBus)
        {
            _bus = commandBus;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet(nameof(Error))]
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
