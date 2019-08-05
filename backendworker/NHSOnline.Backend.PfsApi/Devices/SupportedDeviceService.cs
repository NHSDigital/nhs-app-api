using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Areas.Configuration;
using NHSOnline.Backend.Support.Settings;
using static NHSOnline.Backend.Support.Constants;

namespace NHSOnline.Backend.PfsApi.Devices
{
    public class SupportedDeviceService : ISupportedDeviceService
    {
        private readonly ILogger<SupportedDeviceService> _logger;
        private readonly Dictionary<string, string> SupportedAppVersions;
        private bool _throttlingEnabled;
        private Uri _fidoServerUrl;

        public SupportedDeviceService(ILogger<SupportedDeviceService> logger, DeviceConfigurationSettings settings)
        {
            _logger = logger;

            SupportedAppVersions = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                { SupportedDeviceNames.Android, settings.MinimumSupportedAndroidVersion },
                { SupportedDeviceNames.iOS, settings.MinimumSupportediOSVersion }
            };

            CheckIfThrottlingEnabled(settings);

            GetFidoServerUrl(settings);
        }

        private void CheckIfThrottlingEnabled(DeviceConfigurationSettings settings)
        {
            if (!bool.TryParse(settings.ThrottlingEnabled, out _throttlingEnabled))
            {
                throw new ConfigurationNotFoundException();
            }
        }

        private void GetFidoServerUrl(DeviceConfigurationSettings settings)
        {
            _fidoServerUrl = settings.FidoServerUrl;
        }

        [AllowAnonymous]
        public GetConfigurationResult IsDeviceSupported(DeviceDetails device)
        {
            if (string.IsNullOrEmpty(device.Name) || string.IsNullOrEmpty(device.NativeAppVersion))
            {
                _logger.LogError($"{nameof(device.Name)} or {nameof(device.NativeAppVersion)} is null or empty");
                return new GetConfigurationResult.BadRequest();
            }

            _logger.LogDebug($"Checking if device name {device.Name} is valid");

            if (!SupportedAppVersions.ContainsKey(device.Name))
            {
                _logger.LogError($"Device name {device.Name} not recognised:");
                return new GetConfigurationResult.BadRequest();
            }

            var minimumSupportedVersionForDevice = SupportedAppVersions[device.Name];

            if (string.IsNullOrEmpty(minimumSupportedVersionForDevice))
            {
                return new GetConfigurationResult.InternalServerError();
            }

            if (!Version.TryParse(device.NativeAppVersion, out Version actualNativeAppVersion))
            {
                _logger.LogError($"Couldn't parse native app version: {device.NativeAppVersion}");
                return new GetConfigurationResult.BadRequest();
            }

            if (!Version.TryParse(minimumSupportedVersionForDevice, out Version actualMinimumVersion))
            {
                _logger.LogError($"Couldn't parse minimum supported version: {minimumSupportedVersionForDevice}");
                return new GetConfigurationResult.InternalServerError();
            }

            _logger.LogInformation(
                $"Checking if {device.Name} device with native app version {device.NativeAppVersion} is supported - minimum version is {minimumSupportedVersionForDevice}");

            var result = actualNativeAppVersion.CompareTo(actualMinimumVersion);
            if (result < 0)
            {
                _logger.LogInformation(
                    $"App version {device.NativeAppVersion} is less than minimum supported version {minimumSupportedVersionForDevice}");
                return new GetConfigurationResult.Success(isDeviceSupported: false,
                    isThrottlingEnabled: _throttlingEnabled,
                    fidoServerUrl: _fidoServerUrl);
            }

            return new GetConfigurationResult.Success(isDeviceSupported: true,
                isThrottlingEnabled: _throttlingEnabled,
                fidoServerUrl: _fidoServerUrl);
        }
    }
}
