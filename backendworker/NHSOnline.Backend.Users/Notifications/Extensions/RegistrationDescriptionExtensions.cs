using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.Users.Notifications.Extensions
{
    public static class RegistrationDescriptionExtensions
    {
        private const string InstallationTagName = "$InstallationId:";
        private static readonly int InstallationIdGuidLength = Guid.Empty.ToString().Length;

        public static string[] InstallationIds(this IEnumerable<RegistrationDescription> registrations)
        {
            return registrations.SelectMany(rd => rd.Tags)
                .Where(t => t.StartsWith(InstallationTagName, StringComparison.OrdinalIgnoreCase))
                .Select(t => t.Substring(InstallationTagName.Length + 1, InstallationIdGuidLength))
                .Distinct()
                .ToArray();
        }
    }
}
