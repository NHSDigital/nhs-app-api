namespace NHSOnline.Backend.UsersApi.Areas.Devices.Models
{
    public class MigrationRequest
    {
        public string DevicePns { get; set; }
        public DeviceType DeviceType { get; set; }
        public string NhsLoginId { get; set; }
        public string InstallationId { get; set; }
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }

        public override string ToString() => $"installation id {InstallationId} for nhs login id {NhsLoginId} from {SourcePath} to {TargetPath}";
    }
}
