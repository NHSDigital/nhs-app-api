using NHSOnline.App.Services.UserPreferences.Models;

namespace NHSOnline.App.Services.UserPreferences
{
    internal interface IUserPreferencesService
    {
        bool ShowGettingStarted { get; set; }

        string? BiometricsKeyId { get; set; }

        string FidoUsername { get; set; }

        NotificationsRegistration GetNotificationsRegistration(string nhsLoginId);

        void SetNotificationsRegistration(string nhsLoginId, NotificationsRegistration registration);
    }
}