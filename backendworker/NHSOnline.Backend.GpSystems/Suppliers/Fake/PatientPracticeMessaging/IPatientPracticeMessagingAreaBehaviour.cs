using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientPracticeMessaging
{
    [FakeGpArea("PatientPracticeMessaging")]
    public interface IPatientPracticeMessagingAreaBehaviour
    {
        Task<GetPatientMessagesResult> GetMessages(GpUserSession gpUserSession);
        Task<GetPatientMessageResult> GetMessageDetails(string messageId, GpUserSession gpUserSession);
        Task<PostPatientMessageResult> SendMessage(GpUserSession gpUserSession, CreatePatientMessage message);
        Task<DeletePatientMessageResult> DeleteMessage(GpUserSession gpUserSession, string messageId);
        Task<PutPatientMessageReadStatusResult> UpdateMessageMessageReadStatus(GpUserSession gpUserSession, UpdateMessageReadStatusRequestBody updateRequest);
        Task<GetPatientMessageRecipientsResult> GetMessageRecipients();
    }
}