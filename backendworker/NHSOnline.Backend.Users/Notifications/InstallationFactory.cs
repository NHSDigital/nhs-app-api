using System;
using System.Collections.Generic;
using Microsoft.Azure.NotificationHubs;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Notifications.Models;

namespace NHSOnline.Backend.Users.Notifications
{
    public class InstallationFactory : IInstallationFactory
    {
        private readonly IInstallationTemplateFactory _installationTemplateFactory;
        private const string PrimaryMessageTemplateName = "PrimaryMessageTemplate";

        public InstallationFactory(IInstallationTemplateFactory installationTemplateFactory)
        {
            _installationTemplateFactory = installationTemplateFactory;
        }

        public Installation Create(InstallationRequest request)
        {
            return new Installation
            {
                InstallationId = Guid.NewGuid().ToString(),
                PushChannel = request.DevicePns,
                Tags = new List<string> { NhsLoginTagGenerator.Generate(request.NhsLoginId) },
                Platform = GetNotificationPlatform(request.DeviceType),
                Templates = GetNotificationTemplates(request.DeviceType, request.NhsLoginId)
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