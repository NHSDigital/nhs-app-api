using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Router.Prescriptions;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.Router
{
    public interface IPrescriptionService
    {
        Task<GetPrescriptionsResult> Get(UserSession userSession, DateTimeOffset? fromDate, DateTimeOffset? toDate);
    }
}
