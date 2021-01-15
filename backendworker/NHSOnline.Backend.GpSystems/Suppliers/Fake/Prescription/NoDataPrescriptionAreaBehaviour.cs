using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescription
{
    [FakeGpAreaBehaviour(Behaviour.NoData)]
    public class NoDataPrescriptionAreaBehaviour : IPrescriptionAreaBehaviour
    {

        private readonly FilteringCounts _filteringCounts = new FilteringCounts
        {
            ReceivedCount = 0,
            ReturnedCount = 0,
            ReceivedRepeatsCount = 0,
            ExcessRepeatsCount = 0,
        };

        private readonly PrescriptionListResponse _prescriptionsListResponse = new PrescriptionListResponse
        {
            Prescriptions = new List<PrescriptionItem>(),
            Courses = new List<Course>(),
        };

        public async Task<GetPrescriptionsResult> GetPrescriptions(GpLinkedAccountModel gpLinkedAccountModel,
            DateTimeOffset? fromDate = null,  DateTimeOffset? toDate = null)
        {
            return await Task.FromResult<GetPrescriptionsResult>(
                new GetPrescriptionsResult.Success(_prescriptionsListResponse, _filteringCounts));
        }

        public async Task<OrderPrescriptionResult> OrderPrescription(GpLinkedAccountModel gpLinkedAccountModel,
            RepeatPrescriptionRequest repeatPrescriptionRequest)
        {
            return await Task.FromResult<OrderPrescriptionResult>(new OrderPrescriptionResult.Success());
        }
    }
}