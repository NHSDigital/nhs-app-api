using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;

namespace NHSOnline.Backend.GpSystems.CreateIm1Connection
{
    public interface ICreateIm1ConnectionService
    {
        Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request, IGpSystem gpSystem);

    }
}