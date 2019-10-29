using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public interface IPrescriptionService
    {
        Task<GetPrescriptionsResult> GetPrescriptions(GpLinkedAccountModel gpLinkedAccountModel, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null);
        
        Task<OrderPrescriptionResult> OrderPrescription(GpLinkedAccountModel gpLinkedAccountModel, RepeatPrescriptionRequest repeatPrescriptionRequest);
    }
}
