using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.Router.Prescriptions
{
    public interface IPrescriptionService
    {
        Task<PrescriptionResult> Get(UserSession userSession, DateTimeOffset? fromDate, DateTimeOffset? toDate);
        
        Task<PrescriptionResult> Post(UserSession userSession, RepeatPrescriptionRequest repeatPrescriptionRequest);
    }
}
