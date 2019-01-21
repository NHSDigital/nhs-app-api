using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public interface IVisionPFSClient
    {
        Task<VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(
            VisionConnectionToken token,
            string odsCode);

        Task<VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(
            VisionUserSession userSession);

        Task<VisionPFSClient.VisionApiObjectResponse<PrescriptionHistoryResponse>> GetHistoricPrescriptions(
            VisionUserSession userSession,
            PrescriptionRequest prescriptionRequest);
    
        Task<VisionPFSClient.VisionApiObjectResponse<VisionDemographicsResponse>> GetDemographics(
            VisionUserSession visionUserSession,
            DemographicsRequest requestContent);
        
        Task<VisionPFSClient.VisionApiObjectResponse<EligibleRepeatsResponse>> GetEligibleRepeats(
            VisionUserSession session);

        Task<VisionPFSClient.VisionApiObjectResponse<OrderNewPrescriptionResponse>> OrderNewPrescription(
            VisionUserSession userSession,
            OrderNewPrescriptionRequest newPrescriptionRequest);

        Task<VisionPFSClient.VisionApiObjectResponse<BookedAppointmentsResponse>> GetExistingAppointments(
            VisionUserSession userSession);
        
        Task<VisionPFSClient.VisionApiObjectResponse<AvailableAppointmentsResponse>> GetAvailableAppointments(
            VisionUserSession visionUserSession, AppointmentSlotsDateRange dateRange);
        
        Task<VisionPFSClient.VisionApiObjectResponse<BookAppointmentResponse>> BookAppointment(
            VisionUserSession userSession, BookAppointmentRequest bookAppointmentRequest);
        
        Task<VisionPFSClient.VisionApiObjectResponse<CancelledAppointmentResponse>> CancelAppointment(
            VisionUserSession userSession, CancelAppointmentRequest request);
            
        Task<VisionPFSClient.VisionApiObjectResponse<VisionPatientDataResponse>> GetPatientData(
            VisionUserSession visionUserSession,
            PatientDataRequest requestContent);
        
        Task<VisionPFSClient.VisionApiObjectResponse<ServiceContentRegisterResponse>> PostLinkAccount(string odsCode,
            PatientIm1ConnectionRequest request, string dob);
    }
}
