using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Repository
{
    public class MockUserDeviceRepository : IUserDeviceRepository
    {
        public Task Create(UserDevice userDevice, RegisterDeviceRequest request)
        {
            //Mock just returns a result
            return Task.CompletedTask;
        }
    }
}