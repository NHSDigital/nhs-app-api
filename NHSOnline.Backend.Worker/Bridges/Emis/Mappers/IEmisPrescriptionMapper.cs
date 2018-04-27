using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Mappers
{
    public interface IEmisPrescriptionMapper
    {
        PrescriptionListResponse Map(PrescriptionRequestsGetResponse prescriptionGetResponse);
    }
}
