using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.AccountAggregate.Commands
{
    public class CreateAccountCommand : Command
    {
        public CreateAccountCommand(Guid accountId, Guid userId, long number) : base(accountId)
        {
            UserId = userId;
            Number = number;
        }

        public Guid AccountId => AggregateId;
        public Guid UserId { get; }
        public long Number { get; }
    }
}