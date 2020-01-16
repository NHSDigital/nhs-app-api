using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Messages
{
    public interface IPatientMessagesService
    {
        Task<GetPatientMessagesResult> GetMessages(GpUserSession gpUserSession);
        Task<GetPatientMessageResult> GetMessageDetails(string messageId, GpUserSession gpUserSession);
        Task<PutPatientMessageReadStatusResult> UpdateMessageMessageReadStatus(GpUserSession gpUserSession,
            UpdateMessageReadStatusRequestBody updateRequest);
        Task<GetPatientMessageRecipientsResult> GetMessageRecipients(GpUserSession gpUserSession);
    }
}