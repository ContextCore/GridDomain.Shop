using GridDomain.Node;
using Serilog;
using Serilog.Events;

namespace Shop.Node {
    class ShopLogConfiguration : DefaultLoggerConfiguration
    {
        private const string DefaultTemplate = "[{Level:u3}] [TH{Thread}] [{Timestamp:dd.MM.yyyy HH:mm:ss.fff}] [Src:{LogSource}]"
                                              + "{NewLine} {Message}"
                                              + "{NewLine} {Exception}";

        public ShopLogConfiguration(LogEventLevel level = LogEventLevel.Verbose)
        {
            WriteTo.Console(level,DefaultTemplate);
            WriteTo.RollingFile(".\\Logs.Shop.Node\\shop_node_log_{HalfHour}.txt", level, DefaultTemplate);
        }
    }
}