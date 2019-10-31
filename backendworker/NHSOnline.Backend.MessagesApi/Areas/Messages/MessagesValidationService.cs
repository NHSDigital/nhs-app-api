using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    
    public interface IMessagesValidationService
    {
        bool IsPatchRequestValid(JsonPatchDocument<Message> jsonPatch, string messageId);
        bool IsMessageRequestValid(AddMessageRequest addMessageRequest, string nhsLoginId);
    }
    
    public class MessagesValidationService: IMessagesValidationService
    {
        private readonly ILogger<IMessagesValidationService> _logger;

        public MessagesValidationService(ILogger<IMessagesValidationService> logger)
        {
            _logger = logger;
        }

        public bool IsPatchRequestValid(JsonPatchDocument<Message> jsonPatch, string messageId)
        {
            return new ValidateAndLog(_logger)
                .IsListPopulated(jsonPatch.Operations, nameof(jsonPatch.Operations))
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
    }
}