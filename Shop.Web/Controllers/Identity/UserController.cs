using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GridDomain.CQRS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.UserAggregate.Commands;
using Shop.Infrastructure;
using Shop.ReadModel.Context;
using Shop.ReadModel.Queries;
using Shop.Web.Controllers.Domain;
using Shop.Web.Identity;
using Shop.Web.Identity.ViewModels;

namespace Shop.Web.Controllers
{
    [Route(Routes.Api.User.Controller)]
    public class UserController : Controller
    {
        private readonly ShopIdentityDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        private readonly ICommandExecutor _commandBus;
        private readonly ISequenceProvider _accountNumberProvider;
        private readonly ISingleQuery<Guid, User> _userQuery;
        private const string AccountSequenceName = "AccountSequence";

        public UserController(UserManager<AppUser> userManager,
                              IMapper mapper,
                              ShopIdentityDbContext appDbContext,
                              ICommandExecutor commandBus,
                              ISequenceProvider accountNumberProvider,
                              IUserInfoQuery userQuery)
        {
            _userQuery = userQuery;
            _userManager = userManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
            _commandBus = commandBus;
            _accountNumberProvider = accountNumberProvider;
        }

        [HttpPost(Routes.Api.User.Create)]
        public async Task<IActionResult> Create([FromBody] RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = _mapper.Map<AppUser>(model);
            var result = await _userManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded)
                Errors.AddErrorsToModelState(result, ModelState);

            await _appDbContext.SaveChangesAsync();


            var createUserCommand = new CreateUserCommand(Guid.NewGuid(), model.Email, Guid.NewGuid());
            var accountNumber = _accountNumberProvider.GetNext(AccountSequenceName);
            var createAccountCommand = new CreateAccountCommand(createUserCommand.AccountId, createUserCommand.UserId, accountNumber);
            await _commandBus.Execute(createUserCommand, createAccountCommand);

            return new ContentResult {Content = JsonConvert.SerializeObject(new UserCreatedViewModel(createUserCommand.UserId, createAccountCommand.AccountId, accountNumber))};
        }


        [HttpGet(Routes.Api.User.Get)]
        public async Task<UserViewModel> Get(Guid id)
        {
            var user = await _userQuery.Execute(id);
            return new UserViewModel() {Login = user.Login, Created = user.Created};
        }
    }
}