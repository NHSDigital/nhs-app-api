using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public interface IVisionDirectServicesClient
    {
        Task<VisionDirectServicesApiObjectResponse<PatientConfigurationResponse>> GetConfigurationV2(
            VisionConnectionToken token, string odsCode);

        Task<VisionDirectServicesApiObjectResponse<PatientConfigurationResponse>> GetConfigurationV2(
            VisionUserSession userSession);
    }
}
