using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.AccountAggregate.Events
{
    public class AccountCreated : DomainEvent
    {
        public AccountCreated(Guid sourceId, Guid userId, long accountNumber) : base(sourceId)
        {
            AccountNumber = accountNumber;
            UserId = userId;
        }

        public long AccountNumber { get; }
        public Guid UserId { get; }
    }
}