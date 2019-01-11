using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Areas.Configuration;
using NHSOnline.Backend.Worker.Settings;
using static NHSOnline.Backend.Worker.Areas.Configuration.GetConfigurationResult;
using static NHSOnline.Backend.Worker.Constants;

namespace NHSOnline.Backend.Worker.Support.Devices
{
    public class SupportedDeviceService : ISupportedDeviceService
    {
        private readonly ILogger<SupportedDeviceService> _logger;
        private readonly Dictionary<string, string> SupportedAppVersions;
        private bool _throttlingEnabled;
        private Uri _fidoServerUrl;

        public SupportedDeviceService(ILogger<SupportedDeviceService> logger, IOptions<ConfigurationSettings> settings)
        {
            _logger = logger;

            SupportedAppVersions = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                { SupportedDeviceNames.Android, settings.Value.MinimumSupportedAndroidVersion },
                { SupportedDeviceNames.iOS, settings.Value.MinimumSupportediOSVersion }
            };

            CheckIfThrottlingEnabled(settings);

            GetFidoServerUrl(settings);
        }

        private void CheckIfThrottlingEnabled(IOptions<ConfigurationSettings> settings)
        {
            if (!bool.TryParse(settings.Value.ThrottlingEnabled, out _throttlingEnabled))
            {
                throw new ConfigurationNotFoundException();
            }
        }

        private void GetFidoServerUrl(IOptions<ConfigurationSettings> settings)
        {
            _fidoServerUrl = settings.Value.FidoServerUrl;
        }

        [AllowAnonymous]
        public GetConfigurationResult IsDeviceSupported(DeviceDetails device)
        {
            if (string.IsNullOrEmpty(device.Name) || string.IsNullOrEmpty(device.NativeAppVersion))
            {
                _logger.LogError($"{nameof(device.Name)} or {nameof(device.NativeAppVersion)} is null or empty");
                return new MissingDetailsResult();
            }

            string minimumSupportedVersionForDevice = null;

            _logger.LogDebug($"Checking if device name {device.Name} is valid");

            if (!SupportedAppVersions.ContainsKey(device.Name))
            {
                _logger.LogError($"Device name {device.Name} not recognised:");
                return new InvalidDeviceNameResult();
            }

            minimumSupportedVersionForDevice = SupportedAppVersions[device.Name];

            if (string.IsNullOrEmpty(minimumSupportedVersionForDevice))
            {
                return new ErrorRetrievingConfigResult();
            }

            if (!Version.TryParse(device.NativeAppVersion, out Version actualNativeAppVersion))
            {
                _logger.LogError($"Couldn't parse native app version: {device.NativeAppVersion}");
                return new InvalidNativeAppVersionResult();
            }

            if (!Version.TryParse(minimumSupportedVersionForDevice, out Version actualMinimumVersion))
            {
                _logger.LogError($"Couldn't parse minimum supported version: {minimumSupportedVersionForDevice}");
                return new ErrorRetrievingConfigResult();
            }

            _logger.LogInformation(
                $"Checking if {device.Name} device with native app version {device.NativeAppVersion} is supported - minimum version is {minimumSupportedVersionForDevice}");

            var result = actualNativeAppVersion.CompareTo(actualMinimumVersion);
            if (result < 0)
            {
                _logger.LogInformation(
                    $"App version {device.NativeAppVersion} is less than minimum supported version {minimumSupportedVersionForDevice}");
                return new SuccessfullyRetrieved(isDeviceSupported: false,
                    isThrottlingEnabled: _throttlingEnabled,
                    fidoServerUrl: _fidoServerUrl);
            }

            return new SuccessfullyRetrieved(isDeviceSupported: true,
                isThrottlingEnabled: _throttlingEnabled,
                fidoServerUrl: _fidoServerUrl);
        }
    }
}
