using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.VisionServiceDefinition;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    /// <summary>
    /// Patient facing services client.
    /// </summary>
    internal sealed class VisionPfsClient : IVisionPfsClient
    {
        private readonly string _providerId;
        private readonly ILogger<VisionPfsClient> _logger;
        private readonly VisionConfigurationSettings _settings;
        private readonly VisionPfsClientRequestSender _requestSender;

        public VisionPfsClient(
            ILogger<VisionPfsClient> logger,
            VisionConfigurationSettings settings,
            VisionPfsClientRequestSender requestSender)
        {
            _logger = logger;
            _settings = settings;
            _requestSender = requestSender;

            _providerId = _settings.ApplicationProviderId;
        }

        public async Task<VisionPfsApiObjectResponse<VisionDemographicsResponse>> GetDemographics(
            VisionUserSession visionUserSession,
            DemographicsRequest requestContent)
        {
            var visionServiceDefinition = new DemographicsServiceDefinition();

            return await _requestSender.Post<VisionDemographicsResponse, DemographicsRequest>(
                visionServiceDefinition,
                visionUserSession,
                requestContent);
        }

        public async Task<VisionPfsApiObjectResponse<VisionPatientDataResponse>> GetPatientData(
            VisionUserSession visionUserSession, PatientDataRequest requestContent)
        {
            var visionServiceDefinition = new PatientDataServiceDefinition();

            return await _requestSender.Post<VisionPatientDataResponse, PatientDataRequest>(
                visionServiceDefinition,
                visionUserSession,
                requestContent);
        }

        public async Task<VisionPfsApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(
            VisionConnectionToken token, string odsCode)
        {
            new ValidateAndLog(_logger).IsNotNullOrWhitespace(odsCode, nameof(odsCode), ThrowError)
                .IsNotNull(token, nameof(token), ThrowError)
                .IsNotNullOrWhitespace(token?.RosuAccountId, nameof(VisionConnectionToken.RosuAccountId), ThrowError)
                .IsNotNullOrWhitespace(token?.ApiKey, nameof(VisionConnectionToken.ApiKey), ThrowError)
                .IsValid();

            var visionServiceDefinition = new ConfigurationServiceDefinition();

            var visionRequest = new VisionRequest<object>(
                visionServiceDefinition.Name,
                visionServiceDefinition.Version,
                token.RosuAccountId,
                token.ApiKey,
                odsCode,
                _providerId,
                null);

            return await _requestSender.Post<PatientConfigurationResponse, object>(visionRequest);
        }

        public async Task<VisionPfsApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(VisionUserSession userSession)
        {
            var visionServiceDefinition = new ConfigurationServiceDefinition();

            var visionRequest = new VisionRequest<object>(
                visionServiceDefinition.Name,
                visionServiceDefinition.Version,
                userSession.RosuAccountId,
                userSession.ApiKey,
                userSession.OdsCode,
                _providerId,
                null);

            return await _requestSender.Post<PatientConfigurationResponse, object>(visionRequest);
        }

        public async Task<VisionPfsApiObjectResponse<EligibleRepeatsResponse>> GetEligibleRepeats(
            VisionUserSession session)
        {
            var visionServiceDefinition = new EligibleRepeatsServiceDefinition();

            var requestContent = new CoursesRequest { PatientId = session.PatientId };

            return await _requestSender.Post<EligibleRepeatsResponse, CoursesRequest>(
                visionServiceDefinition,
                session,
                requestContent);
        }

        public async Task<VisionPfsApiObjectResponse<PrescriptionHistoryResponse>> GetHistoricPrescriptions(
            VisionUserSession userSession, PrescriptionRequest prescriptionRequest)
        {
            var visionServiceDefinition = new PrescriptionHistoryServiceDefinition();

            return await _requestSender.Post<PrescriptionHistoryResponse, PrescriptionRequest>(
                visionServiceDefinition,
                userSession,
                prescriptionRequest);
        }

        public async Task<VisionPfsApiObjectResponse<OrderNewPrescriptionResponse>> OrderNewPrescription(
            VisionUserSession userSession, OrderNewPrescriptionRequest newPrescriptionRequest)
        {
            var visionServiceDefinition = new NewPrescriptionServiceDefinition();

            return await _requestSender.Post<OrderNewPrescriptionResponse, OrderNewPrescriptionRequest>(
                visionServiceDefinition,
                userSession,
                newPrescriptionRequest);
        }

        public async Task<VisionPfsApiObjectResponse<BookedAppointmentsResponse>> GetExistingAppointments(
            VisionUserSession userSession
        )
        {
            var visionServiceDefinition = new GetExistingAppointmentsServiceDefinition();

            return await _requestSender.Post<BookedAppointmentsResponse, PatientId>(
                visionServiceDefinition,
                userSession,
                new PatientId { Id = userSession.PatientId });
        }

        public async Task<VisionPfsApiObjectResponse<AvailableAppointmentsResponse>> GetAvailableAppointments(
            VisionUserSession visionUserSession, AppointmentSlotsDateRange dateRange)
        {
            var visionServiceDefinition = new GetAvailableAppointmentsServiceDefinition();

            var request = new AvailableAppointmentsRequest
            {
                PatientId = visionUserSession.PatientId,

                Page = new Page
                {
                    Number = 1,
                    SlotsPerPage = _settings.VisionAppointmentSlotsRequestCount
                },
                Locations = visionUserSession.LocationIds,
                DateRange = new DateRange
                {
                    From = dateRange.FromDate.Date,
                    To = dateRange.ToDate.Date.AddDays(-1)
                }
            };

            return await _requestSender.Post<AvailableAppointmentsResponse, AvailableAppointmentsRequest>(
                visionServiceDefinition, visionUserSession, request);
        }

        public async Task<VisionPfsApiObjectResponse<BookAppointmentResponse>> BookAppointment(
            VisionUserSession userSession,
            BookAppointmentRequest bookAppointmentRequest
        )
        {
            var visionServiceDefinition = new BookAppointmentServiceDefinition();

            return await _requestSender.Post<BookAppointmentResponse, BookAppointmentRequest>(
                visionServiceDefinition, userSession, bookAppointmentRequest);
        }

        public async Task<VisionPfsApiObjectResponse<CancelledAppointmentResponse>> CancelAppointment(
            VisionUserSession userSession,
            CancelAppointmentRequest request
        )
        {
            var visionServiceDefinition = new CancelAppointmentServiceDefinition();

            return await _requestSender.Post<CancelledAppointmentResponse, CancelAppointmentRequest>(
                visionServiceDefinition,
                userSession,
                request);
        }

        public async Task<VisionPfsApiObjectResponse<ServiceContentRegisterResponse>> PostLinkAccount(string odsCode,
            PatientIm1ConnectionRequest request, string dob)
        {
            var visionServiceDefinition = new RegisterServiceDefinition();

            var visionRequest = new VisionRequest<object>(
                visionServiceDefinition.Name,
                visionServiceDefinition.Version,
                odsCode,
                _providerId,
                request.AccountId,
                request.LinkageKey,
                request.Surname,
                dob);

            return await _requestSender.Post<ServiceContentRegisterResponse, object>(visionRequest);
        }
    }
}
