using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    [FakeGpArea("Prescription")]
    public interface IPrescriptionAreaBehaviour
    {
        Task<GetPrescriptionsResult> GetPrescriptions(GpLinkedAccountModel gpLinkedAccountModel,
            DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null);

        Task<OrderPrescriptionResult> OrderPrescription(GpLinkedAccountModel gpLinkedAccountModel,
            RepeatPrescriptionRequest repeatPrescriptionRequest);
    }
}