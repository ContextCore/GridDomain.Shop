using System;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using Serilog;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.Domain.DomainServices.PriceCalculator;

namespace Shop.Domain.ProcessManagers
{
    public class BuyNowProcessStateFactory : IProcessStateFactory<BuyNowState>
    {
        public BuyNowState Create(object message)
        {
            switch (message)
            {
                case SkuPurchaseOrdered e: return new BuyNowState(Guid.NewGuid(), nameof(BuyNow.Initial));
            }
            throw new InvalidMessageToCreateStateException();
        }
    }
}