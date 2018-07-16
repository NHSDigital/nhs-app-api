using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public interface IVisionClient
    {
        Task<VisionClient.VisionApiObjectResponse<PatientConfiguration>> GetConfiguration(VisionConnectionToken token,
            string odsCode);
    }
}
