using System;
using System.Collections.Generic;
using Microsoft.Azure.NotificationHubs;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public class InstallationFactory : IInstallationFactory
    {
        private readonly IInstallationTemplateFactory _installationTemplateFactory;
        private const string PrimaryMessageTemplateName = "PrimaryMessageTemplate";

        public InstallationFactory(IInstallationTemplateFactory installationTemplateFactory)
        {
            _installationTemplateFactory = installationTemplateFactory;
        }

        public Installation Create(string devicePns, DeviceType deviceType, string nhsLoginId)
        {
            return new Installation
            {
                InstallationId = Guid.NewGuid().ToString(),
                PushChannel = devicePns,
                Tags = new List<string> { NhsLoginTagGenerator.Generate(nhsLoginId) },
                Platform = GetNotificationPlatform(deviceType),
                Templates = GetNotificationTemplates(deviceType, nhsLoginId)
            };
        }

        private static NotificationPlatform GetNotificationPlatform(DeviceType deviceType)
        {
            return deviceType switch
            {
                DeviceType.Android => NotificationPlatform.Fcm,
                DeviceType.Ios => NotificationPlatform.Apns,
                _ => throw new ArgumentOutOfRangeException(nameof(deviceType))
            };
        }

        private Dictionary<string, InstallationTemplate> GetNotificationTemplates(DeviceType deviceType, string nhsLoginId)
        {
            return new Dictionary<string, InstallationTemplate>
            {
                { $"{nhsLoginId}-{PrimaryMessageTemplateName}", _installationTemplateFactory.Create(deviceType) }
            };
        }
    }
}