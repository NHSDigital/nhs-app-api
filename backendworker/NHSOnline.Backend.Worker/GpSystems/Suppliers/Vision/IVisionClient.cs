using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

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
    }
}
