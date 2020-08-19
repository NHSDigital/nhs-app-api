using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    [FakeGpAreaBehaviour(Behaviour.Forbidden)]
    public class ForbiddenPrescriptionAreaBehaviour : IPrescriptionAreaBehaviour
    {
        public Task<GetPrescriptionsResult> GetPrescriptions(GpLinkedAccountModel gpLinkedAccountModel, DateTimeOffset? fromDate = null,
            DateTimeOffset? toDate = null) => Task.FromResult<GetPrescriptionsResult>(new GetPrescriptionsResult.Forbidden());

        public Task<OrderPrescriptionResult> OrderPrescription(GpLinkedAccountModel gpLinkedAccountModel, RepeatPrescriptionRequest repeatPrescriptionRequest)
            => Task.FromResult<OrderPrescriptionResult>(new OrderPrescriptionResult.Forbidden());
    }
}