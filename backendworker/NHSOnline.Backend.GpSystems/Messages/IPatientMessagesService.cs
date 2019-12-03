using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Messages
{
    public interface IPatientMessagesService
    {
        Task<GetPatientMessagesResult> GetMessages(GpUserSession gpUserSession);
    }
}