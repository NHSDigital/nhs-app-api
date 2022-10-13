using NHSOnline.Backend.Users.Areas.Devices.Models;

namespace NHSOnline.Backend.Users.Notifications.Models
{
    public class InstallationRequest
    {
        public string DevicePns { get; set; }
        public DeviceType DeviceType { get; set; }
        public string NhsLoginId { get; set; }
        public string InstallationId { get; set; }

        public InstallationRequest()
        {
        }

        public InstallationRequest(MigrationRequest request)
        {
            DevicePns = request.DevicePns;
            DeviceType = request.DeviceType;
            NhsLoginId = request.NhsLoginId;
            InstallationId = request.InstallationId;
        }
    }
}
