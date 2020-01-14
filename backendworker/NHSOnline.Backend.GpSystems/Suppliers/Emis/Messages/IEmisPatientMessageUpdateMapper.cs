using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public interface IEmisPatientMessageUpdateMapper
    {
        PutPatientMessageUpdateStatusResponse Map(MessageUpdateResponse response);
    }
}