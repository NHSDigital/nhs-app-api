using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Verifications;
using static NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.EmisClient;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public interface IEmisClient
    {
        // Demographics
        Task<EmisApiObjectResponse<DemographicsGetResponse>> DemographicsGet(string userPatientLinkToken,
            string responseSessionId,
            string endUserSessionId);

        // Me
        Task<EmisApiObjectResponse<MeApplicationsPostResponse>> MeApplicationsPost(string endUserSessionId,
            MeApplicationsPostRequest model);

        // Sessions
        Task<EmisApiObjectResponse<SessionsEndUserSessionPostResponse>> SessionsEndUserSessionPost();

        Task<EmisApiObjectResponse<SessionsPostResponse>> SessionsPost(string endUserSessionId,
            SessionsPostRequest model);

        // Prescriptions
        Task<EmisApiObjectResponse<PrescriptionRequestsGetResponse>> PrescriptionsGet(
            string userPatientLinkToken, string responseSessionId, string endUserSessionId,
            DateTimeOffset? fromDateTime, DateTimeOffset? toDateTime);

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
            EmisHeaderParameters headerParameters, string userPatientLinkToken, bool fetchPreviousAppointments,
            DateTimeOffset? previousAppointmentsFromDate);

        Task<EmisApiObjectResponse<CancelAppointmentDeleteResponse>> AppointmentsDelete(
            EmisHeaderParameters headerParameters,
            CancelAppointmentDeleteRequest deleteRequest);

        // Linkage
        Task<EmisApiObjectResponse<AddVerificationResponse>> VerificationPost(EmisHeaderParameters headerParameters, AddVerificationRequest addVerificationRequest);

        Task<EmisApiObjectResponse<AddNhsUserResponse>> NhsUserPost(EmisHeaderParameters headerParameters, AddNhsUserRequest addNhsUserRequest);
    }
}