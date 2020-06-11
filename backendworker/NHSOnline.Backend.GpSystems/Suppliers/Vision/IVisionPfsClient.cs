using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public interface IVisionPfsClient
    {
        Task<VisionPfsApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(
            VisionConnectionToken token,
            string odsCode);

        Task<VisionPfsApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(
            VisionUserSession userSession);

        Task<VisionPfsApiObjectResponse<PrescriptionHistoryResponse>> GetHistoricPrescriptions(
            VisionUserSession userSession,
            PrescriptionRequest prescriptionRequest);
    
        Task<VisionPfsApiObjectResponse<VisionDemographicsResponse>> GetDemographics(
            VisionUserSession visionUserSession,
            DemographicsRequest requestContent);
        
        Task<VisionPfsApiObjectResponse<EligibleRepeatsResponse>> GetEligibleRepeats(
            VisionUserSession session);

        Task<VisionPfsApiObjectResponse<OrderNewPrescriptionResponse>> OrderNewPrescription(
            VisionUserSession userSession,
            OrderNewPrescriptionRequest newPrescriptionRequest);

        Task<VisionPfsApiObjectResponse<BookedAppointmentsResponse>> GetExistingAppointments(
            VisionUserSession userSession);
        
        Task<VisionPfsApiObjectResponse<AvailableAppointmentsResponse>> GetAvailableAppointments(
            VisionUserSession visionUserSession, AppointmentSlotsDateRange dateRange);
        
        Task<VisionPfsApiObjectResponse<BookAppointmentResponse>> BookAppointment(
            VisionUserSession userSession, BookAppointmentRequest bookAppointmentRequest);
        
        Task<VisionPfsApiObjectResponse<CancelledAppointmentResponse>> CancelAppointment(
            VisionUserSession userSession, CancelAppointmentRequest request);
            
        Task<VisionPfsApiObjectResponse<VisionPatientDataResponse>> GetPatientData(
            VisionUserSession visionUserSession,
            PatientDataRequest requestContent);
        
        Task<VisionPfsApiObjectResponse<ServiceContentRegisterResponse>> PostLinkAccount(string odsCode,
            PatientIm1ConnectionRequest request, string dob);
    }
}
