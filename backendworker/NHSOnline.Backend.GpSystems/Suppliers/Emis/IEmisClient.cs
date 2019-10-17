using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using static NHSOnline.Backend.GpSystems.Suppliers.Emis.EmisClient;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public interface IEmisClient
    {
        // Demographics
        Task<EmisApiObjectResponse<DemographicsGetResponse>> DemographicsGet(EmisHttpRequestData emisHttpRequestData);

        // Me
        Task<EmisApiObjectResponse<MeApplicationsPostResponse>> MeApplicationsPost(string endUserSessionId,
            MeApplicationsPostRequest model);

        Task<EmisApiObjectResponse<MeSettingsGetResponse>> MeSettingsGet(string userPatientLinkToken, EmisHeaderParameters headerParameters);

        // Sessions
        Task<EmisApiObjectResponse<SessionsEndUserSessionPostResponse>> SessionsEndUserSessionPost();

        Task<EmisApiObjectResponse<SessionsPostResponse>> SessionsPost(string endUserSessionId,
            SessionsPostRequest model);

        // Prescriptions
        Task<EmisApiObjectResponse<PrescriptionRequestsGetResponse>> PrescriptionsGet(
            string userPatientLinkToken, string responseSessionId, string endUserSessionId,
            DateTimeOffset? fromDateTime, DateTimeOffset? toDateTime);

        Task<EmisApiObjectResponse<IndividualDocument>> MedicalDocumentGet(string userPatientLinkToken,
            string responseSessionId, string documentGuid, string endUserSessionId);

        Task<EmisApiObjectResponse<PrescriptionRequestPostResponse>> PrescriptionsPost(
            string responseSessionId, string endUserSessionId, PrescriptionRequestsPost model);

        Task<EmisApiObjectResponse<MedicationRootObject>> MedicalRecordGet(string userPatientLinkToken, string responseSessionId,
            string endUserSessionId, RecordType recordType);

        // Patient Record
        //        Task<EmisClient.EmisApiObjectResponse<AllergyRequestsGetResponse>> AllergiesGet(string userPatientLinkToken, string responseSessionId,
        //            string endUserSessionId);

        //        Task<EmisClient.EmisApiObjectResponse<MedicationRootObject>> MedicationsGet(string userPatientLinkToken, string responseSessionId,
        //            string endUserSessionId);

        //        Task<EmisClient.EmisApiObjectResponse<ImmunisationRequestsGetResponse>> ImmunisationsGet(string userPatientLinkToken, string responseSessionId,
        //            string endUserSessionId);

        //        Task<EmisClient.EmisApiObjectResponse<TestResultRequestsGetResponse>> TestResultsGet(string userPatientLinkToken, string responseSessionId,
        //            string endUserSessionId);

        // PracticeSettings
        Task<EmisApiObjectResponse<PracticeSettingsGetResponse>> PracticeSettingsGet(
             EmisHeaderParameters headerParameters, string practiceCode);

        // AppointmentSlots
        Task<EmisApiObjectResponse<AppointmentSlotsGetResponse>> AppointmentSlotsGet(
            EmisHeaderParameters headerParameters, SlotsGetQueryParameters queryParameters);

        // AppointmentSlotsMetadata
        Task<EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>> AppointmentSlotsMetadataGet(
            EmisHeaderParameters headerParameters, SlotsMetadataGetQueryParameters queryParameters);

        // Courses
        Task<EmisApiObjectResponse<CoursesGetResponse>> CoursesGet(string userPatientLinkToken,
            string responseSessionId, string endUserSessionId);

        // Appointments
        Task<EmisApiObjectResponse<BookAppointmentSlotPostResponse>> AppointmentsPost(
            EmisHeaderParameters headerParameters,
            BookAppointmentSlotPostRequest postRequest);

        Task<EmisApiObjectResponse<AppointmentsGetResponse>> AppointmentsGet(
            EmisHeaderParameters headerParameters, string userPatientLinkToken);

        Task<EmisApiObjectResponse<CancelAppointmentDeleteResponse>> AppointmentsDelete(
            EmisHeaderParameters headerParameters,
            CancelAppointmentDeleteRequest deleteRequest);

        // Linkage
        Task<EmisApiObjectResponse<AddVerificationResponse>> VerificationPost(
            EmisHeaderParameters headerParameters, AddVerificationRequest addVerificationRequest);

        Task<EmisApiObjectResponse<AddNhsUserResponse>> NhsUserPost(
            EmisHeaderParameters headerParameters, AddNhsUserRequest addNhsUserRequest);
    }
}