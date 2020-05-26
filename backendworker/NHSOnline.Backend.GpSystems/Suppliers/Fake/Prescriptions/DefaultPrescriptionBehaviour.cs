using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    public class DefaultPrescriptionBehaviour : IPrescriptionBehaviour
    {
        public async Task<GetPrescriptionsResult> GetPrescriptions(GpLinkedAccountModel gpLinkedAccountModel,
            DateTimeOffset? fromDate = null,  DateTimeOffset? toDate = null)
        {
            return await Task.FromResult<GetPrescriptionsResult>(new GetPrescriptionsResult.Forbidden());
        }

        public async Task<OrderPrescriptionResult> OrderPrescription(GpLinkedAccountModel gpLinkedAccountModel,
            RepeatPrescriptionRequest repeatPrescriptionRequest)
        {
            return await Task.FromResult<OrderPrescriptionResult>(new OrderPrescriptionResult.Forbidden());
        }
    }
}