using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientMessageRecipientsPost : ITppClientRequest<TppUserSession, MessageRecipientsReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;
        private readonly ILogger<TppClientMessageRecipientsPost> _logger;

        public TppClientMessageRecipientsPost(
            ILogger<TppClientMessageRecipientsPost> logger,
            TppClientRequestExecutor requestExecutor)
        {
            _logger = logger;
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<MessageRecipientsReply>> Post(TppUserSession tppUserSession)
        {
            _logger.LogEnter();

            try
            {
                var request = new MessageRecipients
                {
                    PatientId = tppUserSession.PatientId,
                    OnlineUserId = tppUserSession.OnlineUserId,
                    UnitId = tppUserSession.OdsCode,
                };

                return await _requestExecutor.Post<MessageRecipientsReply>(
                    requestBuilder => requestBuilder.Model(request).Suid(tppUserSession.Suid));
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}