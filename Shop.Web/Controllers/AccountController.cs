using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GridDomain.CQRS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Infrastructure;
using Shop.Web.Identity;
using Shop.Web.Identity.ViewModels;

namespace Shop.Web.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly ICommandExecutor _commandBus;

        private readonly ShopIdentityDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, IMapper mapper, ShopIdentityDbContext appDbContext, ICommandExecutor commandBus)
        {
            _userManager = userManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
            _commandBus = commandBus;
        }

      // [HttpPost]
      // public async Task<Guid> Create()
      // {
      //     var accountId = Guid.NewGuid();
      //     var userId = Guid.NewGuid(); //TODO: get current user id
      //     var accountNumber = _serviceProvider.GetNext(AccountSequence);
      //     await _commandBus.Execute(new CreateAccountCommand(accountId, userId, accountNumber));
      //     return accountId;
      // }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RegistrationViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = _mapper.Map<AppUser>(model);
            var result = await _userManager.CreateAsync(userIdentity, model.Password);

            if(!result.Succeeded) 
                        foreach(var e in result.Errors)
                            ModelState.TryAddModelError(e.Code, e.Description);

            await _appDbContext.SaveChangesAsync();

            return new OkResult();
        }

    }
}
