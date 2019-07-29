namespace NHSOnline.Backend.UsersApi.Areas.Devices.Models
{
    public class RegisterDeviceRequest
    {
        public DeviceType? DeviceType { get; set; }
        public string DevicePns { get; set; }
    }
}