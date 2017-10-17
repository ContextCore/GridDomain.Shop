using System.Linq;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace Shop.Tests.Unit {
    public static class AggregateExtensions
    {
        //public static T ApplyEvent<T>(this IAggregate aggregate, T evt) where T:DomainEvent
        //{
        //    aggregate.ApplyEvent(evt);
        //    return evt;
        //}
        //public static void ApplyEvents(this IAggregate aggregate, params DomainEvent[] evts)
        //{
        //    foreach(var e in evts)
        //       aggregate.ApplyEvent(e);
        //}
        //public static T GetEvent<T>(this IAggregate aggregate) where T : DomainEvent
        //{
        //    return aggregate.GetUncommittedEvents().OfType<T>().First();
        //}
    }
}