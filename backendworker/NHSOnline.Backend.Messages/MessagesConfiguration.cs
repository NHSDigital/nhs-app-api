using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Messages
{
    public class MessagesConfiguration : IMessagesConfiguration
    {
        public MessagesConfiguration(IConfiguration configuration, ILogger<MessagesConfiguration> logger)
        {
            SenderIdNhsApp = configuration.GetOrThrow("MESSAGES_SENDER_ID_NHS_APP", logger);
            SupplierIdNhsApp = configuration.GetOrThrow("MESSAGES_SUPPLIER_ID_NHS_APP", logger);
        }

        public string SenderIdNhsApp { get; }

        public string SupplierIdNhsApp { get; }
    }
}