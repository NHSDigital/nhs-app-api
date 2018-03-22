using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;

namespace NHSOnline.Backend.Worker.Router.Im1Connection
{
    public interface IIm1ConnectionService
    {
        Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode);
        Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request);
    }
}