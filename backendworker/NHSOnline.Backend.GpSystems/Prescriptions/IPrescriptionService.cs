using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public interface IPrescriptionService
    {
        Task<PrescriptionResult> GetPrescriptions(GpUserSession gpUserSession, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null);
        
        Task<PrescriptionResult> OrderPrescription(GpUserSession gpUserSession, RepeatPrescriptionRequest repeatPrescriptionRequest);
    }
}
