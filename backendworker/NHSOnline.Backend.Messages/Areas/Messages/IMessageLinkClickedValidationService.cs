using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public interface IMessageLinkClickedValidationService
    {
        bool IsServiceRequestValid(string nhsLoginId, MessageLink messageLink);
    }
}