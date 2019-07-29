using System;
using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Azure
{
    public class MockAzureNotificationHubService : IAzureNotificationHubService
    {
        public async Task<RegistrationResult> Register(RegisterDeviceRequest request)
        {
            return await Task.FromResult(new RegistrationResult.Success(new AzureRegistrationResponse
            {
                RegistrationId = "123456789-2",
                RegistrationExpiry = new DateTime(2030, 12, 25, 0, 0, 0)
            }));
        }
    }
}