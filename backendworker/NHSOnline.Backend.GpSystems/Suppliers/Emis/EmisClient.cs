using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    [SuppressMessage("Microsoft.Maintainability", "CA1506", Justification = "NHSO-10119 Reduce EmisClient class coupling - In Progress")]
    internal sealed class EmisClient : IEmisClient
    {
        private const string MeApplicationsPath = "me/applications";
        private const string MeSettingsPath = "me/settings?userPatientLinkToken={0}";

        private const string SessionsEndUserSessionPath = "sessions/endusersession";
        private const string SessionsPath = "sessions";
        private const string DemographicsPath = "demographics?userPatientLinkToken={0}";
        private const string PatientRecordPath = "record?userPatientLinkToken={0}&itemType={1}";

        private const string AppointmentSlotsMetaPath =
            "appointmentslots/meta?userPatientLinkToken={0}&sessionStartDate={1}&sessionEndDate={2}";

        private const string AppointmentSlotsPath =
            "appointmentslots?userPatientLinkToken={0}&fromDateTime={1}&toDateTime={2}";

        private const string PracticeSettingsPath = "practices/{0}/settings";
        private const string PrescriptionsPath = "prescriptionrequests?userPatientLinkToken={0}{1}{2}";
        private const string PrescriptionsPostPath = "prescriptionrequests";
        private const string CoursesPath = "courses?userPatientLinkToken={0}";
        private const string DocumentPath = "documents/{0}?userPatientLinkToken={1}";
        private const string AppointmentsPath = "appointments";
        private const string MeVerificationsPath = "me/verifications";
        private const string UsersNhsPath = "users/nhs";
        private const string GetMessagesPath = "messages?userPatientLinkToken={0}";
        private const string GetMessageDetailsPath = "messages/{0}/?userPatientLinkToken={1}";
        private const string GetMessageRecipientsPath = "messagerecipients?userPatientLinkToken={0}";
        private const string PutMessageReadStatusUpdate = "messages";
        private const string PostMessagePath = "messages";
        private const string DeleteMessagePath = "messages/{0}";

        private readonly EmisConfigurationSettings _settings;
        private readonly TimeZoneConverter _localTimeZoneConverter;
        private readonly EmisClientRequestSender _emisClientRequestSender;
        private readonly ILogger<EmisClient> _logger;

        public EmisClient(
            EmisConfigurationSettings settings,
            TimeZoneConverter localTimeZoneConverter,
            EmisClientRequestSender emisClientRequestSender,
            ILogger<EmisClient> logger)
        {
            _settings = settings;
            _localTimeZoneConverter = localTimeZoneConverter;
            _emisClientRequestSender = emisClientRequestSender;
            _logger = logger;

            _settings.Validate();
        }

        public async Task<EmisApiObjectResponse<SessionsEndUserSessionPostResponse>> SessionsEndUserSessionPost()
        {
            return await _emisClientRequestSender.SendRequestAndParseResponse<SessionsEndUserSessionPostResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.SessionsEndUserSessionPost)
                    .Post(SessionsEndUserSessionPath)
                    .EmptyBody()
                    .Timeout(_settings.EmisExtendedHttpTimeoutSeconds));
        }

        public async Task<EmisApiObjectResponse<SessionsPostResponse>> SessionsPost(
            string endUserSessionId,
            SessionsPostRequest model)
        {
            return await _emisClientRequestSender.SendRequestAndParseResponse<SessionsPostResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.SessionsPost)
                    .Post(SessionsPath)
                    .Request(model)
                    .EndUserSessionId(endUserSessionId));
        }

        public async Task<EmisApiObjectResponse<DemographicsGetResponse>> DemographicsGet(
            EmisRequestParameters requestParameters)
        {
            _logger.LogInformation("EMIS: Fetching patient demographics");
            
            return await _emisClientRequestSender.SendRequestAndParseResponse<DemographicsGetResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.DemographicsGet)
                    .Get(DemographicsPath, requestParameters.UserPatientLinkToken)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));
        }

        public async Task<EmisApiObjectResponse<MedicationRootObject>> MedicalRecordGet(
            EmisRequestParameters requestParameters,
            RecordType recordType)
        {
            _logger.LogInformation("EMIS: Fetching patient medical record - {0}", recordType.ToString());

            return await _emisClientRequestSender.SendRequestAndParseResponse<MedicationRootObject>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.MedicalRecordGet)
                    .Get(PatientRecordPath, requestParameters.UserPatientLinkToken, recordType.ToString())
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId)
                    .AdditionalSuccessHttpStatusCode(HttpStatusCode.Forbidden));
        }

        public async Task<EmisApiObjectResponse<IndividualDocument>> MedicalDocumentGet(
            string userPatientLinkToken,
            string responseSessionId,
            string documentIdentifier,
            string endUserSessionId)
        {
            _logger.LogInformation("EMIS: Fetching patient document - {0}", documentIdentifier);
            
            return await _emisClientRequestSender.SendRequestAndParseResponse<IndividualDocument>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.MedicalDocumentGet)
                    .Get(DocumentPath, documentIdentifier, userPatientLinkToken)
                    .SessionId(responseSessionId)
                    .EndUserSessionId(endUserSessionId));
        }

        public async Task<EmisApiObjectResponse<MeApplicationsPostResponse>> MeApplicationsPost(
            string endUserSessionId,
            MeApplicationsPostRequest model)
        {
            return await _emisClientRequestSender.SendRequestAndParseResponse<MeApplicationsPostResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.MeApplicationsPost)
                    .Post(MeApplicationsPath)
                    .Request(model)
                    .EndUserSessionId(endUserSessionId)
                    .Timeout(_settings.EmisExtendedHttpTimeoutSeconds));
        }

        public async Task<EmisApiObjectResponse<PrescriptionRequestsGetResponse>> PrescriptionsGet(
            EmisRequestParameters requestParameters,
            DateTimeOffset? fromDateTime,
            DateTimeOffset? toDateTime)
        {
            return await _emisClientRequestSender.SendRequestAndParseResponse<PrescriptionRequestsGetResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.PrescriptionsGet)
                    .Get(
                        PrescriptionsPath,
                        requestParameters.UserPatientLinkToken,
                        AppendIsoDateTimeIfNotNull("filterFromDate", fromDateTime),
                        AppendIsoDateTimeIfNotNull("filterToDate", toDateTime))
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));

            static string AppendIsoDateTimeIfNotNull(string name, DateTimeOffset? dateTimeOffset)
            {
                if (dateTimeOffset.HasValue)
                {
                    var encodeValue = HttpUtility.UrlEncode(dateTimeOffset.Value.ToString("O", CultureInfo.InvariantCulture));
                    return $"&{name}={encodeValue}";
                }

                return string.Empty;
            }
        }

        public async Task<EmisApiObjectResponse<PrescriptionRequestPostResponse>> PrescriptionsPost(
            string responseSessionId,
            string endUserSessionId,
            PrescriptionRequestsPost model)
        {
            return await _emisClientRequestSender.SendRequestAndParseResponse<PrescriptionRequestPostResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.PrescriptionsPost)
                    .Post(PrescriptionsPostPath)
                    .Request(model)
                    .SessionId(responseSessionId)
                    .EndUserSessionId(endUserSessionId));
        }

        public async Task<EmisApiObjectResponse<AppointmentSlotsGetResponse>> AppointmentSlotsGet(
            EmisRequestParameters requestParameters,
            SlotsGetQueryParameters queryParameters)
        {
            var fromDateTime = _localTimeZoneConverter.ToLocalTime(queryParameters.FromDateTime).ToString("s", CultureInfo.InvariantCulture);
            var toDateTime = _localTimeZoneConverter.ToLocalTime(queryParameters.ToDateTime).ToString("s", CultureInfo.InvariantCulture);

            return await _emisClientRequestSender.SendRequestAndParseResponse<AppointmentSlotsGetResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.AppointmentSlotsGet)
                    .Get(AppointmentSlotsPath, requestParameters.UserPatientLinkToken, fromDateTime, toDateTime)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));
        }

        public async Task<EmisApiObjectResponse<PracticeSettingsGetResponse>> PracticeSettingsGet(
            EmisRequestParameters requestParameters,
            string practiceCode)
        {
            return await _emisClientRequestSender.SendRequestAndParseResponse<PracticeSettingsGetResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.PracticeSettingsGet)
                    .Get(PracticeSettingsPath, practiceCode)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));
        }

        public async Task<EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>> AppointmentSlotsMetadataGet(
            EmisRequestParameters requestParameters,
            SlotsMetadataGetQueryParameters queryParameters)
        {
            var fromDateTime = _localTimeZoneConverter.ToLocalTime(queryParameters.SessionStartDate).ToString("s", CultureInfo.InvariantCulture);
            var toDateTime = _localTimeZoneConverter.ToLocalTime(queryParameters.SessionEndDate).ToString("s", CultureInfo.InvariantCulture);

            return await _emisClientRequestSender.SendRequestAndParseResponse<AppointmentSlotsMetadataGetResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.AppointmentSlotsMetadataGet)
                    .Get(AppointmentSlotsMetaPath, requestParameters.UserPatientLinkToken, fromDateTime, toDateTime)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));
        }

        public async Task<EmisApiObjectResponse<CoursesGetResponse>> CoursesGet(
            EmisRequestParameters requestParameters)
        {
            return await _emisClientRequestSender.SendRequestAndParseResponse<CoursesGetResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.CoursesGet)
                    .Get(CoursesPath, requestParameters.UserPatientLinkToken)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));
        }

        public async Task<EmisApiObjectResponse<BookAppointmentSlotPostResponse>> AppointmentsPost(
            EmisRequestParameters requestParameters,
            AppointmentBookRequest bookRequest)
        {
            var postRequest = new BookAppointmentSlotPostRequest(requestParameters.UserPatientLinkToken, bookRequest);

            return await _emisClientRequestSender.SendRequestAndParseResponse<BookAppointmentSlotPostResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.AppointmentsPost)
                    .Post(AppointmentsPath)
                    .Request(postRequest)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));
        }

        public async Task<EmisApiObjectResponse<AppointmentsGetResponse>> AppointmentsGet(
            EmisRequestParameters requestParameters)
        {
            return await _emisClientRequestSender.SendRequestAndParseResponse<AppointmentsGetResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.AppointmentsGet)
                    .Get(
                        "{0}?UserPatientLinkToken={1}&FetchPreviousAppointments=true&PreviousAppointmentsFromDate={2}",
                        AppointmentsPath,
                        requestParameters.UserPatientLinkToken,
                        DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));
        }

        public async Task<EmisApiObjectResponse<CancelAppointmentDeleteResponse>> AppointmentsDelete(
            EmisRequestParameters requestParameters,
            long slotId,
            CancellationReason cancellationReason)
        {
            var deleteRequest = new CancelAppointmentDeleteRequest(
                requestParameters.UserPatientLinkToken,
                cancellationReason.DisplayName,
                slotId);

            return await _emisClientRequestSender.SendRequestAndParseResponse<CancelAppointmentDeleteResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.AppointmentsDelete)
                    .Delete(AppointmentsPath)
                    .Request(deleteRequest)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));
        }

        public async Task<EmisApiObjectResponse<AddVerificationResponse>> VerificationPost(
            EmisRequestParameters requestParameters,
            AddVerificationRequest addVerificationRequest)
        {
            return await _emisClientRequestSender.SendRequestAndParseResponse<AddVerificationResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.VerificationPost)
                    .Post(MeVerificationsPath)
                    .Request(addVerificationRequest)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId)
                    .Timeout(_settings.EmisExtendedHttpTimeoutSeconds)
                    .AdditionalSuccessHttpStatusCode(HttpStatusCode.Conflict));
        }

        public async Task<EmisApiObjectResponse<MeSettingsGetResponse>> MeSettingsGet(
            EmisRequestParameters requestParameters)
        {
            _logger.LogInformation("EMIS: Fetching patient settings");
            
            return await _emisClientRequestSender.SendRequestAndParseResponse<MeSettingsGetResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.MeSettingsGet)
                    .Get(MeSettingsPath, requestParameters.UserPatientLinkToken)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));
        }

        public async Task<EmisApiObjectResponse<AddNhsUserResponse>> NhsUserPost(
            EmisRequestParameters requestParameters,
            AddNhsUserRequest addNhsUserRequest)
        {
            return await _emisClientRequestSender.SendRequestAndParseResponse<AddNhsUserResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.NhsUserPost)
                    .Post(UsersNhsPath)
                    .Request(addNhsUserRequest)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId)
                    .Timeout(_settings.EmisExtendedHttpTimeoutSeconds));
        }

        public async Task<EmisApiObjectResponse<MessagesGetResponse>> PatientMessagesGet(
            EmisRequestParameters requestParameters)
        {
            return await _emisClientRequestSender.SendRequestAndParseResponse<MessagesGetResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.PatientMessagesGet)
                    .Get(GetMessagesPath, requestParameters.UserPatientLinkToken)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));
        }

        public async Task<EmisApiObjectResponse<MessageGetResponse>> PatientMessageDetailsGet(
            string messageId,
            EmisRequestParameters requestParameters)
        {
            return await _emisClientRequestSender.SendRequestAndParseResponse<MessageGetResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.PatientMessageDetailsGet)
                    .Get(GetMessageDetailsPath, messageId, requestParameters.UserPatientLinkToken)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));
        }

        public async Task<EmisApiObjectResponse<MessageUpdateResponse>> PatientMessageUpdatePut(
            EmisRequestParameters requestParameters,
            UpdateMessageReadStatusRequest updateMessageReadStatusRequest)
        {
            return await _emisClientRequestSender.SendRequestAndParseResponse<MessageUpdateResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.PatientMessageUpdatePut)
                    .Put(PutMessageReadStatusUpdate)
                    .Request(updateMessageReadStatusRequest)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId)
                    .Timeout(_settings.EmisExtendedHttpTimeoutSeconds));
        }

        public async Task<EmisApiObjectResponse<MessageRecipientsResponse>> PatientMessageRecipientsGet(
            EmisRequestParameters requestParameters)
        {
            return await _emisClientRequestSender.SendRequestAndParseResponse<MessageRecipientsResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.PatientMessageRecipientsGet)
                    .Get(GetMessageRecipientsPath, requestParameters.UserPatientLinkToken)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));
        }

        public async Task<EmisApiObjectResponse<MessagePostResponse>> PatientMessagePost(
            EmisRequestParameters requestParameters,
            CreatePatientMessage message)
        {
            var sendMessageRequest = new PostMessageRequest(requestParameters.UserPatientLinkToken, message);

            return await _emisClientRequestSender.SendRequestAndParseResponse<MessagePostResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.SendPatientMessagePost)
                    .Post(PostMessagePath)
                    .Request(sendMessageRequest)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));
        }

        public async Task<EmisApiObjectResponse<MessageDeleteResponse>> PatientPracticeMessageDelete(
            EmisRequestParameters requestParameters,
            string messageId)
        {
            var deleteMessageRequest = new MessageDeleteRequest
            {
                UserPatientLinkToken = requestParameters.UserPatientLinkToken
            };

            return await _emisClientRequestSender.SendRequestAndParseResponse<MessageDeleteResponse>(
                request => request
                    .RequestType(RequestsForSuccessOutcome.PatientPracticeMessageDelete)
                    .Delete(DeleteMessagePath, messageId)
                    .Request(deleteMessageRequest)
                    .SessionId(requestParameters.SessionId)
                    .EndUserSessionId(requestParameters.EndUserSessionId));
        }
    }
}
