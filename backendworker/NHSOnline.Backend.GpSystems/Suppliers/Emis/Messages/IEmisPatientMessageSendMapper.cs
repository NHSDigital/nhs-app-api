using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public interface IEmisPatientMessageSendMapper
    {
        Option<PostMessageResponse> Map(MessagePostResponse response);
    }
}