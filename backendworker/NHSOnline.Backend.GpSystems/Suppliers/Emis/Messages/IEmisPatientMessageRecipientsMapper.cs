using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public interface IEmisPatientMessageRecipientsMapper
    {
        MessageRecipientsResponse Map(MessageRecipientsResponse response);
    }
}