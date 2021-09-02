using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public interface IVisionDirectServicesClient
    {
        Task<VisionDirectServicesApiObjectResponse<PatientConfigurationResponse>> GetConfigurationV2(
            VisionConnectionToken token, string odsCode);

        Task<VisionDirectServicesApiObjectResponse<PatientConfigurationResponse>> GetConfigurationV2(
            VisionUserSession userSession);

        Task<VisionDirectServicesApiObjectResponse<VisionDemographicsResponse>> GetDemographicsV2(
            VisionUserSession visionUserSession,
            DemographicsRequest requestContent);

        Task<VisionDirectServicesApiObjectResponse<BookedAppointmentsResponse>> GetExistingAppointmentsV2(
            VisionUserSession visionUserSession);

        Task<VisionDirectServicesApiObjectResponse<EligibleRepeatsResponse>> GetEligibleRepeatsV2(
            VisionUserSession visionUserSession);

        Task<VisionDirectServicesApiObjectResponse<PrescriptionHistoryResponse>> GetPrescriptionHistoryV2(
            VisionUserSession visionUserSession, PrescriptionRequest prescriptionRequest);
    }
}
