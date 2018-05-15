using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Bridges.Emis.Appointments;
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
        
        // AppointmentSlots
        Task<EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>> AppointmentsSlotsGet(
            EmisHeaderParameters headerParameters, SlotsGetQueryParameters queryParameters);
        
        // AppointmentSlotsMetadata
        Task<EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>> AppointmentsSlotsMetadataGet(
            EmisHeaderParameters headerParameters, SlotsMetadataGetQueryParameters queryParameters);
    }
}