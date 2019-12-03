using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;
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

        Task<EmisApiObjectResponse<MeSettingsGetResponse>> MeSettingsGet(EmisRequestParameters requestParameters);

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
        Task<EmisApiObjectResponse<CoursesGetResponse>> CoursesGet(EmisRequestParameters emisRequestParameters);

        // Appointments
        Task<EmisApiObjectResponse<BookAppointmentSlotPostResponse>> AppointmentsPost(
            EmisRequestParameters requestParameters,
            AppointmentBookRequest bookRequest);

        Task<EmisApiObjectResponse<AppointmentsGetResponse>> AppointmentsGet(EmisRequestParameters requestParameters);

        Task<EmisApiObjectResponse<CancelAppointmentDeleteResponse>> AppointmentsDelete(
            EmisRequestParameters requestParameters,
            long slotId,
            CancellationReason cancellationReason);

        // Linkage
        Task<EmisApiObjectResponse<AddVerificationResponse>> VerificationPost(
            EmisRequestParameters requestParameters, AddVerificationRequest addVerificationRequest);

        Task<EmisApiObjectResponse<AddNhsUserResponse>> NhsUserPost(
            EmisRequestParameters requestParameters, AddNhsUserRequest addNhsUserRequest);

        // Practice to Patient Messages
        Task<EmisApiObjectResponse<MessagesGetResponse>> PatientMessagesGet(
            EmisRequestParameters requestParameters);
        
        Task<EmisApiObjectResponse<MessageGetResponse>> PatientMessageDetailsGet(
            string messageId, EmisRequestParameters requestParameters);
    }
}