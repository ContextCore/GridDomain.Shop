using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using NMoneys;
using Shop.Domain.Aggregates.AccountAggregate.Events;

namespace Shop.Domain.Aggregates.AccountAggregate
{
    public class Account : ConventionAggregate
    {
        private Account(Guid id) : base(id)
        {
            Apply<AccountCreated>(e => {
                                      Id = e.SourceId;
                                      UserId = e.UserId;
                                      Number = e.AccountNumber;
                                  });
            Apply<AccountReplenish>(e => Amount += e.Amount);
            Apply<AccountWithdrawal>(e => Amount -= e.Amount);
        }

        public Account(Guid id, Guid userId, long number) : this(id)
        {
            Produce(new AccountCreated(id, userId, number));
        }

        public Guid UserId { get; private set; }
        public Money Amount { get; private set; }
        public long Number { get; private set; }

        public void Replenish(Money m, Guid replenishSource)
        {
            if (m.IsNegative())
                throw new NegativeMoneyException("Cant replenish negative amount of money.");
            Produce(new AccountReplenish(Id, replenishSource, m));
        }

        public void Withdraw(Money m, Guid withdrawSource)
        {
            if (m.IsNegative())
                throw new NegativeMoneyException("Cant withdrawal negative amount of money.");

            if ((Amount - m).IsNegative())
                throw new NotEnoughMoneyException("Dont have enough money to pay for bill");

            Produce(new AccountWithdrawal(Id, withdrawSource, m));
        }
    }
}