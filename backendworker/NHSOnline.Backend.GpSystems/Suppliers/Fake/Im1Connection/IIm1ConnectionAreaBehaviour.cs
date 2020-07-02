using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Models.Im1Connection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Im1Connection
{
    [FakeGpArea("Im1Connection")]
    public interface IIm1ConnectionAreaBehaviour
    {
        Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request,
            FakeConnectionToken connectionToken);

        Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode);
    }
}