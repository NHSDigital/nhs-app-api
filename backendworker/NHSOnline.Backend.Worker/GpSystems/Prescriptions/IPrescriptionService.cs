using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Prescriptions
{
    public interface IPrescriptionService
    {
        Task<PrescriptionResult> GetPrescriptions(UserSession userSession, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null);
        
        Task<PrescriptionResult> OrderPrescription(UserSession userSession, RepeatPrescriptionRequest repeatPrescriptionRequest);
    }
}
