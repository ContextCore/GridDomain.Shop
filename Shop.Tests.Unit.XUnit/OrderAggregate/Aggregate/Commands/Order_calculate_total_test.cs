using System;
using GridDomain.Tests.Common;
using NMoneys;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Infrastructure;
using Xunit;

namespace Shop.Tests.Unit.XUnit.OrderAggregate.Aggregate.Commands
{
   
    public class Order_calculate_total_test
    {
        private AggregateScenario<Order, OrderCommandsHandler> NewScenario()
        {
            return AggregateScenario<Order, OrderCommandsHandler>.New(null,
                                                                      new OrderCommandsHandler(new InMemorySequenceProvider()));
        }

       [Fact]
        public void Given_order_with_items_When_calculate_total_Then_calculate_total_event_is_raised()
        {
            var sourceId = Guid.NewGuid();

            NewScenario()
                .Given(new OrderCreated(sourceId, 123, Guid.NewGuid()),
                       new ItemAdded(sourceId, Guid.NewGuid(), 1, new Money(50), 1))
                .When(new CalculateOrderTotalCommand(sourceId))
                .Then(new OrderTotalCalculated(sourceId, new Money(50)))
                .Run()
                .Check();
        }
    }
}