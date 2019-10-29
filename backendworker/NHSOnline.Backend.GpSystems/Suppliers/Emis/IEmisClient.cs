using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.GpSystems.Suppliers.Emis.EmisClient;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public interface IEmisClient
    {
        // Demographics
        Task<EmisApiObjectResponse<DemographicsGetResponse>> DemographicsGet(EmisRequestParameters emisRequestParameters);

        // Me
        Task<EmisApiObjectResponse<MeApplicationsPostResponse>> MeApplicationsPost(string endUserSessionId,
            MeApplicationsPostRequest model);

        Task<EmisApiObjectResponse<MeSettingsGetResponse>> MeSettingsGet(string userPatientLinkToken, EmisRequestParameters requestParameters);

        // Sessions
        Task<EmisApiObjectResponse<SessionsEndUserSessionPostResponse>> SessionsEndUserSessionPost();

        Task<EmisApiObjectResponse<SessionsPostResponse>> SessionsPost(string endUserSessionId,
            SessionsPostRequest model);

        // Prescriptions
        Task<EmisApiObjectResponse<PrescriptionRequestsGetResponse>> PrescriptionsGet(
            EmisRequestParameters emisRequestParameters, DateTimeOffset? fromDateTime, DateTimeOffset? toDateTime);

        Task<EmisApiObjectResponse<IndividualDocument>> MedicalDocumentGet(string userPatientLinkToken,
            string responseSessionId, string documentGuid, string endUserSessionId);

        Task<EmisApiObjectResponse<PrescriptionRequestPostResponse>> PrescriptionsPost(
            string responseSessionId, string endUserSessionId, PrescriptionRequestsPost model);

        Task<EmisApiObjectResponse<MedicationRootObject>> MedicalRecordGet(EmisRequestParameters emisRequestParameters, RecordType recordType);

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
             EmisRequestParameters requestParameters, string practiceCode);

        // AppointmentSlots
        Task<EmisApiObjectResponse<AppointmentSlotsGetResponse>> AppointmentSlotsGet(
            EmisRequestParameters requestParameters, SlotsGetQueryParameters queryParameters);

        // AppointmentSlotsMetadata
        Task<EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>> AppointmentSlotsMetadataGet(
            EmisRequestParameters requestParameters, SlotsMetadataGetQueryParameters queryParameters);

        // Courses
        Task<EmisApiObjectResponse<CoursesGetResponse>> CoursesGet( EmisRequestParameters emisRequestParameters);

        // Appointments
        Task<EmisApiObjectResponse<BookAppointmentSlotPostResponse>> AppointmentsPost(
            EmisRequestParameters requestParameters,
            BookAppointmentSlotPostRequest postRequest);

        Task<EmisApiObjectResponse<AppointmentsGetResponse>> AppointmentsGet(
            EmisRequestParameters requestParameters, string userPatientLinkToken);

        Task<EmisApiObjectResponse<CancelAppointmentDeleteResponse>> AppointmentsDelete(
            EmisRequestParameters requestParameters,
            CancelAppointmentDeleteRequest deleteRequest);

        // Linkage
        Task<EmisApiObjectResponse<AddVerificationResponse>> VerificationPost(
            EmisRequestParameters requestParameters, AddVerificationRequest addVerificationRequest);

        Task<EmisApiObjectResponse<AddNhsUserResponse>> NhsUserPost(
            EmisRequestParameters requestParameters, AddNhsUserRequest addNhsUserRequest);
    }
}