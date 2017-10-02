using Serilog;
using Serilog.Events;

namespace Shop.Node {
    class ShopLogConfiguration : LoggerConfiguration
    {
        public const string DefaultTemplate = "{Timestamp:yy-MM-dd HH:mm:ss.fff} [{Level:u3} TH{Thread}] Src:{LogSource}"
                                              + "{NewLine} {Message}"
                                              + "{NewLine} {Exception}";

        public ShopLogConfiguration(LogEventLevel level = LogEventLevel.Verbose)
        {
            Enrich.FromLogContext();
            WriteTo.Console();
            WriteTo.RollingFile(".\\Logs\\shop_node_log_{HalfHour}.txt", level);
            MinimumLevel.Is(level);
            //Destructure.ByTransforming<Money>(r => new { r.Amount, r.CurrencyCode });
            //Destructure.ByTransforming<Exception>(r => new { Type = r.GetType(), r.StackTrace });
            //Destructure.ByTransforming<MessageMetadata>(r => new { r.CasuationId, r.CorrelationId });
            //Destructure.ByTransforming<PersistEventPack>(r => new { Size = r.Events.Length });
            //Destructure.ByTransforming<MessageMetadataEnvelop<ICommand>>(r => new { CommandId = r.Message.Id, r.Metadata });
            //Destructure.ByTransforming<MessageMetadataEnvelop<DomainEvent>>(r => new { EventId = r.Message.Id, r.Metadata });
            //Destructure.ByTransforming<AggregateCommandExecutionContext>(r => new { CommandId = r.Command.Id, Metadata = r.CommandMetadata });
            //Destructure.ByTransforming<ProcessTransitComplete>(r => new { Event = r.InitialMessage, ProducedCommandsNum = r.ProducedCommands.Length });
            //Destructure.ByTransforming<CreateNewProcess>(r => new { Event = (r.Message.Message as IHaveId)?.Id ?? r.Message.Message, r.EnforcedId, r.Message.Metadata });
        }
    }
}