using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public interface IEmisPatientMessageRecipientsMapper
    {
        MessageRecipientsGetResponse Map(MessageRecipientsGetResponse response);
    }
}