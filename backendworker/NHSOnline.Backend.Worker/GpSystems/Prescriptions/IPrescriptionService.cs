using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Prescriptions
{
    public interface IPrescriptionService
    {
        Task<PrescriptionResult> GetPrescriptions(GpUserSession gpUserSession, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null);
        
        Task<PrescriptionResult> OrderPrescription(GpUserSession gpUserSession, RepeatPrescriptionRequest repeatPrescriptionRequest);
    }
}
