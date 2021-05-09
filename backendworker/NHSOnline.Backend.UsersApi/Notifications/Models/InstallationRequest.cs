using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Notifications.Models
{
    public class InstallationRequest
    {
        public string DevicePns { get; set; }
        public DeviceType DeviceType { get; set; }
        public string NhsLoginId { get; set; }

        public InstallationRequest()
        {
        }

        public InstallationRequest(MigrationRequest request)
        {
            DevicePns = request.DevicePns;
            DeviceType = request.DeviceType;
            NhsLoginId = request.NhsLoginId;
        }
    }
}
