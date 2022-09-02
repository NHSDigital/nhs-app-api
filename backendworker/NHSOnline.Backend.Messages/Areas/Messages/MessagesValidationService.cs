using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class MessagesValidationService : IMessagesValidationService
    {
        private readonly ILogger<IMessagesValidationService> _logger;
        private const int AllowedOperationsCount = 1;

        public MessagesValidationService(ILogger<IMessagesValidationService> logger)
        {
            _logger = logger;
        }

        public bool IsPatchRequestValid(JsonPatchDocument<Message> jsonPatch, string messageId)
        {
            return new ValidateAndLog(_logger)
                .IsListPopulated(jsonPatch.Operations, nameof(jsonPatch.Operations))
                .IsListValidLength(jsonPatch.Operations, nameof(jsonPatch.Operations), AllowedOperationsCount)
                .IsListValid(jsonPatch.Operations, x => string.IsNullOrWhiteSpace(x.op),
                    nameof(jsonPatch.Operations))
                .IsNotNullOrWhitespace(messageId, nameof(messageId))
                .IsValid();
        }

        public bool IsMessageRequestValid(AddMessageRequest addMessageRequest, string nhsLoginId)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(addMessageRequest?.Sender, nameof(addMessageRequest.Sender))
                .IsNotNullOrWhitespace(addMessageRequest?.Body, nameof(addMessageRequest.Body))
                .IsNotNullOrWhitespace(nhsLoginId, nameof(nhsLoginId))
                .IsValid();
        }

        public bool IsMessageIdValid(string messageId)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(messageId, nameof(messageId))
                .IsValidMongoDbObjectId(messageId, nameof(messageId))
                .IsValid();
        }
    }
}