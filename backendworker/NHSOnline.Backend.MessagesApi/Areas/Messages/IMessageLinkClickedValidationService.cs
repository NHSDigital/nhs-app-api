using System;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public interface IMessageLinkClickedValidationService
    {
        bool IsServiceRequestValid(string nhsLoginId, MessageLink messageLink);
    }
}