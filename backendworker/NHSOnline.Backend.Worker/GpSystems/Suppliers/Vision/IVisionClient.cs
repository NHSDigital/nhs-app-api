using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public interface IVisionClient
    {
        Task<VisionClient.VisionApiObjectResponse<PatientConfigurationResponse>> GetConfiguration(
            VisionConnectionToken token,
            string odsCode);

        Task<VisionClient.VisionApiObjectResponse<PrescriptionHistoryResponse>> GetHistoricPrescriptions(
            VisionUserSession userSession,
            PrescriptionRequest prescriptionRequest);
    
        Task<VisionClient.VisionApiObjectResponse<VisionDemographicsResponse>> GetDemographics(
            VisionUserSession visionUserSession,
            DemographicsRequest requestContent);
        
        Task<VisionClient.VisionApiObjectResponse<EligibleRepeatsResponse>> GetEligibleRepeats(
                VisionUserSession session);

        Task<VisionClient.VisionApiObjectResponse<OrderNewPrescriptionResponse>> OrderNewPrescription(
            VisionUserSession userSession,
            OrderNewPrescriptionRequest newPrescriptionRequest);

        Task<VisionClient.VisionApiObjectResponse<BookedAppointmentsResponse>> GetExistingAppointments(
            VisionConnectionToken token, string odsCode, string patientId);
            
        Task<VisionClient.VisionApiObjectResponse<VisionPatientDataResponse>> GetPatientData(
            VisionUserSession visionUserSession,
            PatientDataRequest requestContent);
        
        Task<VisionClient.VisionApiObjectResponse<ServiceContentRegisterResponse>> PostLinkAccount(string odsCode,
            PatientIm1ConnectionRequest request, string dob);
    }
}
