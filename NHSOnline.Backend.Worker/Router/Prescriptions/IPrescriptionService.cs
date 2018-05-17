using System;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Router.Prescriptions
{
    public interface IPrescriptionService
    {
        Task<GetPrescriptionsResult> Get(UserSession userSession, DateTimeOffset? fromDate, DateTimeOffset? toDate);
    }
}
