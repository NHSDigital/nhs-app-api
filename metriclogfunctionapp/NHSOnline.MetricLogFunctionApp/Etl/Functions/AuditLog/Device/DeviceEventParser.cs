using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Device
{
    public class DeviceEventParser : IAuditLogParser<DeviceMetric>
    {
        private const string OperationFieldValue = "Login_Device";
        private const string DetailsFieldValue = "Device details returned:";
        private const string NhsAppIosIdentifier = "nhsapp-ios";
        private const string NhsAppAndroidIdentifier = "nhsapp-android";
        private const string NhsAppManufacturerIdentifier = "nhsapp-manufacturer";
        private const string NhsAppModelIdentifier = "nhsapp-model";
        private const string NhsAppOSIdentifier = "nhsapp-os";

        public DeviceMetric Parse(AuditRecord source)
        {
            if (!IsDeviceMetric(source)) return null;

            return new DeviceMetric
            {
                Timestamp = source.Timestamp,
                SessionId = source.SessionId,
                AppVersion = GetAppVersionValueFromAuditRecordDetails(source.Details),
                DeviceManufacturer = GetDeviceManufacturerValueFromAuditRecordDetails(source.Details),
                DeviceModel = GetDeviceModelValueFromAuditRecordDetails(source.Details),
                DeviceOS = GetDeviceOSValueFromAuditRecordDetails(source.Details),
                DeviceOSVersion = GetDeviceOSVersionValueFromAuditRecordDetails(source.Details),
                UserAgent = GetUserAgentValueFromAuditRecordDetails(source.Details),
                AuditId = source.AuditId
            };
        }

        private bool IsDeviceMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source.Details != null && source.Details.Contains(DetailsFieldValue);
        }

        private string GetAppVersionValueFromAuditRecordDetails(string details)
        {
            Regex appVersionPattern;
            Match appVersionMatch = null;

            if (details.Contains(NhsAppIosIdentifier))
            {
                appVersionPattern = new Regex($@".*{NhsAppIosIdentifier}\/\s*(?<appVersion>\S*)");
                appVersionMatch = appVersionPattern.Match(details);
            }

            if (details.Contains(NhsAppAndroidIdentifier))
            {
                appVersionPattern = new Regex($@".*{NhsAppAndroidIdentifier}\/\s*(?<appVersion>\S*)");
                appVersionMatch = appVersionPattern.Match(details);
            }

            return appVersionMatch?.Groups["appVersion"].Value;
        }

        private string GetDeviceManufacturerValueFromAuditRecordDetails(string details)
        {
            if (!details.Contains(NhsAppManufacturerIdentifier)) return null;

            var deviceManufacturerPattern = new Regex($@".*{NhsAppManufacturerIdentifier}\/\s*(?<deviceManufacturer>\S*)");
            var deviceManufacturerMatch = deviceManufacturerPattern.Match(details);

            return deviceManufacturerMatch.Groups["deviceManufacturer"].Value;
        }

        private string GetDeviceModelValueFromAuditRecordDetails(string details)
        {
            if (!details.Contains(NhsAppModelIdentifier)) return null;

            var deviceModelPattern = new Regex($@".*{NhsAppModelIdentifier}\/\s*(?<deviceModel>\S*)");
            var deviceModelMatch = deviceModelPattern.Match(details);

            return deviceModelMatch.Groups["deviceModel"].Value;
        }

        private string GetDeviceOSValueFromAuditRecordDetails(string details)
        {
            if (details.Contains(NhsAppIosIdentifier))
            {
                return "ios";
            }

            return details.Contains(NhsAppAndroidIdentifier) ? "android" : null;
        }

        private string GetDeviceOSVersionValueFromAuditRecordDetails(string details)
        {
            if (!details.Contains(NhsAppOSIdentifier)) return null;

            var deviceOSVersionPattern = new Regex($@".*{NhsAppOSIdentifier}\/\s*(?<deviceOSVersion>\S*)");
            var deviceOSVersionMatch = deviceOSVersionPattern.Match(details);

            return deviceOSVersionMatch.Groups["deviceOSVersion"].Value;
        }

        private string GetUserAgentValueFromAuditRecordDetails(string details)
        {
            return details.Replace(DetailsFieldValue, string.Empty).Trim();
        }
    }
}
