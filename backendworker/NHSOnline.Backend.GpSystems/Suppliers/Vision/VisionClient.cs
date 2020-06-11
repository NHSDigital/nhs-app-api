using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public class VisionClient : IVisionClient
    {
        private readonly IVisionLinkageClient _visionLinkageClient;
        private readonly IVisionPfsClient _visionPfsClient;

        public VisionClient(
            IVisionLinkageClient visionLinkageClient,
            IVisionPfsClient visionPfsClient)
        {
            _visionLinkageClient = visionLinkageClient;
            _visionPfsClient = visionPfsClient;
        }
        
        public Task<VisionLinkageApiObjectResponse<LinkageKeyGetResponse>> GetLinkageKey(GetLinkageKey getLinkageKey)
        {
            return _visionLinkageClient.GetLinkageKey(getLinkageKey);
        }

        public async Task<VisionLinkageApiObjectResponse<LinkageKeyPostResponse>> CreateLinkageKey(CreateLinkageKey createLinkageKey)
        {
            return await _visionLinkageClient.CreateLinkageKey(createLinkageKey);
        }

        public Task<VisionPfsApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(VisionConnectionToken token, string odsCode)
        {
            return _visionPfsClient.GetConfiguration(token, odsCode);
        }

        public Task<VisionPfsApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(VisionUserSession userSession)
        {
            return _visionPfsClient.GetConfiguration(userSession);
        }

        public Task<VisionPfsApiObjectResponse<PrescriptionHistoryResponse>> GetHistoricPrescriptions(VisionUserSession userSession, PrescriptionRequest prescriptionRequest)
        {
            return _visionPfsClient.GetHistoricPrescriptions(userSession, prescriptionRequest);
        }

        public Task<VisionPfsApiObjectResponse<VisionDemographicsResponse>> GetDemographics(VisionUserSession visionUserSession, DemographicsRequest requestContent)
        {
            return _visionPfsClient.GetDemographics(visionUserSession, requestContent);
        }

        public Task<VisionPfsApiObjectResponse<EligibleRepeatsResponse>> GetEligibleRepeats(VisionUserSession session)
        {
            return _visionPfsClient.GetEligibleRepeats(session);
        }

        public Task<VisionPfsApiObjectResponse<OrderNewPrescriptionResponse>> OrderNewPrescription(VisionUserSession userSession, OrderNewPrescriptionRequest newPrescriptionRequest)
        {
            return _visionPfsClient.OrderNewPrescription(userSession, newPrescriptionRequest);
        }

        public Task<VisionPfsApiObjectResponse<BookedAppointmentsResponse>> GetExistingAppointments(VisionUserSession userSession)
        {
            return _visionPfsClient.GetExistingAppointments(userSession);
        }

        public Task<VisionPfsApiObjectResponse<AvailableAppointmentsResponse>> GetAvailableAppointments(VisionUserSession visionUserSession, AppointmentSlotsDateRange dateRange)
        {
            return _visionPfsClient.GetAvailableAppointments(visionUserSession, dateRange);
        }

        public Task<VisionPfsApiObjectResponse<BookAppointmentResponse>> BookAppointment(VisionUserSession userSession, BookAppointmentRequest bookAppointmentRequest)
        {
            return _visionPfsClient.BookAppointment(userSession, bookAppointmentRequest);
        }

        public Task<VisionPfsApiObjectResponse<CancelledAppointmentResponse>> CancelAppointment(VisionUserSession userSession, CancelAppointmentRequest request)
        {
            return _visionPfsClient.CancelAppointment(userSession, request);
        }

        public Task<VisionPfsApiObjectResponse<VisionPatientDataResponse>> GetPatientData(VisionUserSession visionUserSession, PatientDataRequest requestContent)
        {
            return _visionPfsClient.GetPatientData(visionUserSession, requestContent);
        }

        public Task<VisionPfsApiObjectResponse<ServiceContentRegisterResponse>> PostLinkAccount(string odsCode, PatientIm1ConnectionRequest request, string dob)
        {
            return _visionPfsClient.PostLinkAccount(odsCode, request, dob);
        }
    }
}