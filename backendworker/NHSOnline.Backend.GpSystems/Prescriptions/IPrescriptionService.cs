using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public interface IPrescriptionService
    {
        Task<GetPrescriptionsResult> GetPrescriptions(GpUserSession gpUserSession, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null);
        
        Task<OrderPrescriptionResult> OrderPrescription(GpUserSession gpUserSession, RepeatPrescriptionRequest repeatPrescriptionRequest);
    }
}
