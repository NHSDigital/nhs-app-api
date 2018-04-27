using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public interface IEmisClient
    {
        // Demographics
        Task<EmisClient.EmisApiObjectResponse<DemographicsGetResponse>> DemographicsGet(string userPatientLinkToken, string responseSessionId,
            string endUserSessionId);

        // Me
        Task<EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>> MeApplicationsPost(string endUserSessionId, MeApplicationsPostRequest model);

        // Sessions
        Task<EmisClient.EmisApiObjectResponse<SessionsEndUserSessionPostResponse>> SessionsEndUserSessionPost();
        Task<EmisClient.EmisApiObjectResponse<SessionsPostResponse>> SessionsPost(string endUserSessionId, SessionsPostRequest model);

        // Prescriptions
        Task<EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>> PrescriptionsGet(
            string userPatientLinkToken, string responseSessionId, string endUserSessionId, DateTimeOffset? fromDateTime, DateTimeOffset? toDate);
    }
}