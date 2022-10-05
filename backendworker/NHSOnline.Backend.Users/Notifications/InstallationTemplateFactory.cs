using System;
using Microsoft.Azure.NotificationHubs;
using NHSOnline.Backend.Users.Areas.Devices.Models;

namespace NHSOnline.Backend.Users.Notifications
{
    internal class InstallationTemplateFactory : IInstallationTemplateFactory
    {
        private const string AppleTemplate = "{\"aps\": {\"alert\": { \"title\" : \"$(title)\", \"subtitle\" : \"$(subtitle)\", \"body\" : \"$(body)\" }, \"url\" : \"$(url)\", \"sound\" : \"default\", \"badge\": \"#(badgeCount)\" }}";
        private const string AndroidTemplate = "{\"notification\": { \"title\" : \"$(title)\", \"body\" : \"$(body)\" }, \"data\":{ \"url\" : \"$(url)\"}}";

        public InstallationTemplate Create(DeviceType deviceType)
        {
            return deviceType switch
            {
                DeviceType.Android => new InstallationTemplate { Body = AndroidTemplate },
                DeviceType.Ios => new InstallationTemplate { Body = AppleTemplate },
                _ => throw new ArgumentOutOfRangeException(nameof(deviceType), deviceType, null)
            };
        }
    }
}