using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using Microsoft.AspNetCore.Mvc;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.UserAggregate.Commands;
using Shop.Infrastructure;

namespace Shop.Web.Controllers.Domain {

    [Route(Routes.Api.User.Controller)]
    public class UserController : Controller
    {
        private readonly ICommandExecutor _commandBus;
        private readonly ISequenceProvider _accountNumberProvider;
        private const string AccountSequenceName = "AccountSequence";
        public UserController(ICommandExecutor commandBus, ISequenceProvider accountNumberProvider)
        {
            _accountNumberProvider = accountNumberProvider;
            _commandBus = commandBus;
        }

        [HttpPost(Routes.Api.User.Create)]
        public async Task<UserCreatedViewModel> CreateUser([FromBody] CreateUserViewModel userViewModel)
        {
            var createUserCommand = new CreateUserCommand(Guid.NewGuid(), userViewModel.Login, Guid.NewGuid());
            var accountNumber = _accountNumberProvider.GetNext(AccountSequenceName);
            var createAccountCommand = new CreateAccountCommand(createUserCommand.AccountId, createUserCommand.UserId, accountNumber);
            await _commandBus.Execute(createUserCommand);
            await _commandBus.Execute(createAccountCommand);
            return new UserCreatedViewModel(createUserCommand.UserId, createAccountCommand.AccountId, accountNumber);
        }
    }
}