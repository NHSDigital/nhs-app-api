using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientMessagesViewPost : ITppClientRequest<TppUserSession, MessagesViewReply>
    {
        private readonly ILogger<TppClientMessagesViewPost> _logger;
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientMessagesViewPost(
            ILogger<TppClientMessagesViewPost> logger,
            TppClientRequestExecutor requestExecutor)
        {
            _logger = logger;
            _requestExecutor = requestExecutor;
        }

        public Task<TppApiObjectResponse<MessagesViewReply>> Post(TppUserSession tppUserSession)
        {
            var request = new MessagesView
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.OdsCode,
            };

            return _requestExecutor.Post<MessagesViewReply>(
                requestBuilder => requestBuilder.Model(request).Suid(tppUserSession.Suid));

        }
    }
}