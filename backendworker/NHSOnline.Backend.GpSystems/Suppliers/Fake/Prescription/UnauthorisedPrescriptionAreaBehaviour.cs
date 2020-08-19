using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    [FakeGpAreaBehaviour(Behaviour.Unauthorised)]
    public class UnauthorisedPrescriptionAreaBehaviour: IPrescriptionAreaBehaviour
    {
        public Task<GetPrescriptionsResult> GetPrescriptions(GpLinkedAccountModel gpLinkedAccountModel,
            DateTimeOffset? fromDate = null,  DateTimeOffset? toDate = null)
        {
            throw new UnauthorisedGpSystemHttpRequestException();
        }

        public Task<OrderPrescriptionResult> OrderPrescription(GpLinkedAccountModel gpLinkedAccountModel, RepeatPrescriptionRequest repeatPrescriptionRequest)
        {
            throw new UnauthorisedGpSystemHttpRequestException();
        }
    }
}