using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class VisionClient : IVisionClient
    {
        private readonly IVisionLinkageClient _visionLinkageClient;
        private readonly IVisionPFSClient _visionPFSClient;

        public VisionClient(
            IVisionLinkageClient visionLinkageClient,
            IVisionPFSClient visionPFSClient)
        {
            _visionLinkageClient = visionLinkageClient;
            _visionPFSClient = visionPFSClient;
        }
        
        public Task<VisionLinkageClient.VisionApiObjectResponse<LinkageKeyGetResponse>> GetLinkageKey(GetLinkageKey getLinkageKey)
        {
            return _visionLinkageClient.GetLinkageKey(getLinkageKey);
        }

        public async Task<VisionLinkageClient.VisionApiObjectResponse<LinkageKeyPostResponse>> CreateLinkageKey(CreateLinkageKey createLinkageKey)
        {
            return await _visionLinkageClient.CreateLinkageKey(createLinkageKey);
        }

        public Task<VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(VisionConnectionToken token, string odsCode)
        {
            return _visionPFSClient.GetConfiguration(token, odsCode);
        }

        public Task<VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(VisionUserSession userSession)
        {
            return _visionPFSClient.GetConfiguration(userSession);
        }

        public Task<VisionPFSClient.VisionApiObjectResponse<PrescriptionHistoryResponse>> GetHistoricPrescriptions(VisionUserSession userSession, PrescriptionRequest prescriptionRequest)
        {
            return _visionPFSClient.GetHistoricPrescriptions(userSession, prescriptionRequest);
        }

        public Task<VisionPFSClient.VisionApiObjectResponse<VisionDemographicsResponse>> GetDemographics(VisionUserSession visionUserSession, DemographicsRequest requestContent)
        {
            return _visionPFSClient.GetDemographics(visionUserSession, requestContent);
        }

        public Task<VisionPFSClient.VisionApiObjectResponse<EligibleRepeatsResponse>> GetEligibleRepeats(VisionUserSession session)
        {
            return _visionPFSClient.GetEligibleRepeats(session);
        }

        public Task<VisionPFSClient.VisionApiObjectResponse<OrderNewPrescriptionResponse>> OrderNewPrescription(VisionUserSession userSession, OrderNewPrescriptionRequest newPrescriptionRequest)
        {
            return _visionPFSClient.OrderNewPrescription(userSession, newPrescriptionRequest);
        }

        public Task<VisionPFSClient.VisionApiObjectResponse<BookedAppointmentsResponse>> GetExistingAppointments(VisionUserSession userSession)
        {
            return _visionPFSClient.GetExistingAppointments(userSession);
        }

        public Task<VisionPFSClient.VisionApiObjectResponse<AvailableAppointmentsResponse>> GetAvailableAppointments(VisionUserSession visionUserSession, AppointmentSlotsDateRange dateRange)
        {
            return _visionPFSClient.GetAvailableAppointments(visionUserSession, dateRange);
        }

        public Task<VisionPFSClient.VisionApiObjectResponse<BookAppointmentResponse>> BookAppointment(VisionUserSession userSession, BookAppointmentRequest bookAppointmentRequest)
        {
            return _visionPFSClient.BookAppointment(userSession, bookAppointmentRequest);
        }

        public Task<VisionPFSClient.VisionApiObjectResponse<CancelledAppointmentResponse>> CancelAppointment(VisionUserSession userSession, CancelAppointmentRequest request)
        {
            return _visionPFSClient.CancelAppointment(userSession, request);
        }

        public Task<VisionPFSClient.VisionApiObjectResponse<VisionPatientDataResponse>> GetPatientData(VisionUserSession visionUserSession, PatientDataRequest requestContent)
        {
            return _visionPFSClient.GetPatientData(visionUserSession, requestContent);
        }

        public Task<VisionPFSClient.VisionApiObjectResponse<ServiceContentRegisterResponse>> PostLinkAccount(string odsCode, PatientIm1ConnectionRequest request, string dob)
        {
            return _visionPFSClient.PostLinkAccount(odsCode, request, dob);
        }
    }
}