using Microsoft.AspNetCore.JsonPatch;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public interface IMessagesValidationService
    {
        bool IsPatchRequestValid(JsonPatchDocument<Message> jsonPatch, string messageId);
        bool IsMessageRequestValid(AddMessageRequest addMessageRequest, string nhsLoginId);
        bool IsMessageIdValid(string messageId);
    }
}
