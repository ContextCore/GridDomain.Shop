using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;
using Microsoft.AspNetCore.Mvc;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Infrastructure;

namespace Shop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICommandExecutor _commandBus;
        private readonly ISequenceProvider _serviceProvider;
        private const string AccountSequence = nameof(AccountSequence);
        public AccountController(ICommandExecutor commandBus, ISequenceProvider provider)
        {
            _serviceProvider = provider;
            _commandBus = commandBus;
        }

        [HttpPost]
        public async Task<Guid> Create()
        {
            var accountId = Guid.NewGuid();
            var userId = Guid.NewGuid(); //TODO: get current user id
            var accountNumber = _serviceProvider.GetNext(AccountSequence);
            await _commandBus.Execute(new CreateAccountCommand(accountId, userId, accountNumber));
            return accountId;
        }
    }
}
