using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Messages
{
    public class MessagesConfiguration : IMessagesConfiguration
    {
        public MessagesConfiguration(IConfiguration configuration, ILogger<MessagesConfiguration> logger)
        {
            SenderIdEnabled = configuration.GetBoolOrThrow("MESSAGES_SENDER_ID_ENABLED", logger);
            SenderIdNhsApp = configuration.GetOrThrow("MESSAGES_SENDER_ID_NHS_APP", logger);
        }

        public bool SenderIdEnabled { get; }

        public string SenderIdNhsApp { get; }
    }
}