using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientMessagesSendMessagePost:
        ITppClientRequest<(TppUserSession tppUserSession, string recipientIdentifier,
            string messageText), MessageCreateReply>
    {
        private static readonly Regex IdentifierRegex = new Regex(Constants.Regex.IdentifierRegex);
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientMessagesSendMessagePost(TppClientRequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<MessageCreateReply>> Post(
            (TppUserSession tppUserSession, string recipientIdentifier,
                string messageText) parameters)
        {
            var (tppUserSession, recipientIdentifier, messageText) = parameters;
            var request = new MessageCreate
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.OdsCode,
                Message = messageText
            };

            ParseRecipientIdentifier(request, recipientIdentifier);

            return await _requestExecutor.Post<MessageCreateReply>(
                requestBuilder => requestBuilder.Model(request).Suid(tppUserSession.Suid));
        }

        private static void ParseRecipientIdentifier(MessageCreate request, string recipientIdentifier)
        {
            if (!IdentifierRegex.IsMatch(recipientIdentifier))
            {
                throw new ArgumentException("Recipient identifier not in the correct format");
            }

            var recipientIdentifierSplit = recipientIdentifier.Split(":");

            if (recipientIdentifierSplit[1] == "UnitRecipient")
            {
                request.UnitRecipientId = recipientIdentifierSplit[0];
            }
            else
            {
                request.RecipientId = recipientIdentifierSplit[0];
            }
        }
    }
}