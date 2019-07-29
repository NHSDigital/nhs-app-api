using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Azure;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public interface IDeviceRepositoryService
    {
        Task<DeviceRepositoryResult> Create(
            AzureRegistrationResponse registration,
            RegisterDeviceRequest request);
    }
}