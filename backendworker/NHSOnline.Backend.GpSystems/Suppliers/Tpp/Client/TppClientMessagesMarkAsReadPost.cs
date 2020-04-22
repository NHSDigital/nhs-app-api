using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientMessagesMarkAsReadPost :
        ITppClientRequest<(TppRequestParameters, List<string>), MessagesMarkAsReadReply>
    {
        private readonly ITppClientRequestExecutor _requestExecutor;

        public TppClientMessagesMarkAsReadPost(
            ITppClientRequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public Task<TppApiObjectResponse<MessagesMarkAsReadReply>> Post(
            (TppRequestParameters,  List<string>) parameters)
        {
            var (requestParameters, messageIdsToMarkAsRead) = parameters;

            if (requestParameters is null)
            {
                throw new ArgumentNullException($"First item in {nameof(parameters)} tuple " +
                                                $"must not be null");
            }

            var messagesToMarkAsRead = messageIdsToMarkAsRead?.Select(id => new Message
            {
                MessageId = id
            }).ToList() ?? new List<Message>();

            var request = new MessagesMarkAsRead()
            {
                PatientId = requestParameters.PatientId,
                OnlineUserId = requestParameters.OnlineUserId,
                UnitId = requestParameters.OdsCode,
                Messages = messagesToMarkAsRead
            };

            return _requestExecutor.Post<MessagesMarkAsReadReply>(
                requestBuilder => requestBuilder.Model(request).Suid(requestParameters.Suid));
        }
    }
}