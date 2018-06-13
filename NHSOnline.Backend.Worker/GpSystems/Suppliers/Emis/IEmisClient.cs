using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.AppointmentSlots;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public interface IEmisClient
    {
        // Demographics
        Task<EmisClient.EmisApiObjectResponse<DemographicsGetResponse>> DemographicsGet(string userPatientLinkToken,
            string responseSessionId,
            string endUserSessionId);

        // Me
        Task<EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>> MeApplicationsPost(string endUserSessionId,
            MeApplicationsPostRequest model);

        // Sessions
        Task<EmisClient.EmisApiObjectResponse<SessionsEndUserSessionPostResponse>> SessionsEndUserSessionPost();

        Task<EmisClient.EmisApiObjectResponse<SessionsPostResponse>> SessionsPost(string endUserSessionId,
            SessionsPostRequest model);

        // Prescriptions
        Task<EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>> PrescriptionsGet(
            string userPatientLinkToken, string responseSessionId, string endUserSessionId,
            DateTimeOffset? fromDateTime, DateTimeOffset? toDate);

        Task<EmisClient.EmisApiObjectResponse<PrescriptionRequestPostResponse>> PrescriptionsPost(
            string responseSessionId, string endUserSessionId, PrescriptionRequestsPost model);
        
        // Patient Record
        Task<EmisClient.EmisApiObjectResponse<AllergyRequestsGetResponse>> AllergiesGet(string userPatientLinkToken, string responseSessionId,
            string endUserSessionId);
        
        Task<EmisClient.EmisApiObjectResponse<MedicationRequestsGetResponse>> MedicationsGet(string userPatientLinkToken, string responseSessionId,
            string endUserSessionId);
        
        // AppointmentSlots
        Task<EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>> AppointmentsSlotsGet(
            EmisHeaderParameters headerParameters, SlotsGetQueryParameters queryParameters);

        // AppointmentSlotsMetadata
        Task<EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>> AppointmentsSlotsMetadataGet(
            EmisHeaderParameters headerParameters, SlotsMetadataGetQueryParameters queryParameters);

        // Courses
        Task<EmisClient.EmisApiObjectResponse<CoursesGetResponse>> CoursesGet(string userPatientLinkToken,
            string responseSessionId, string endUserSessionId);

        Task<EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>> AppointmentPost(
            EmisHeaderParameters headerParameters,
            BookAppointmentSlotPostRequest postRequest);
    }
}