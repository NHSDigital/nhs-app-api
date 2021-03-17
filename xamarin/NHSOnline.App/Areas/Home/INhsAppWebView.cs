using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.Home
{
    internal interface INhsAppWebView: INavigationView<INhsAppWebView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? Appearing { get; set; }
            Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
            Func<WebNavigatedEventArgs, Task>? Navigated { get; set; }

            Func<Task>? SettingsRequested { get; set; }
            Func<Task>? HelpRequested { get; set; }
            Func<Task>? HomeRequested { get; set; }
            Func<Task>? SymptomsRequested { get; set; }
            Func<Task>? AppointmentsRequested { get; set; }
            Func<Task>? PrescriptionsRequested { get; set; }
            Func<Task>? RecordRequested { get; set; }
            Func<Task>? MoreRequested { get; set; }

            Func<Task>? ResetAndShowErrorRequested { get; set; }

            Func<OpenWebIntegrationRequest, Task>? OpenWebIntegrationRequested { get; set; }
            Func<StartNhsLoginUpliftRequest, Task>? StartNhsLoginUpliftRequested { get; set; }

            Func<Task>? GetNotificationsStatusRequested { get; set; }
            Func<string, Task>? GetPnsTokenRequested { get; set; }

            Func<Task>? FetchBiometricSpecRequested { get; set; }
            Func<string, Task>? UpdateBiometricRegistrationRequested { get; set; }
        }

        void GoToUri(Uri uri);
        Task NavigateToRedirectedPathWithinApp(string spaPath);
        Task NavigateToAdvice();
        Task NavigateToAppointments();
        Task NavigateToPrescriptions();
        Task NavigateToYourHealth();
        Task NavigateToMessages();
        Task NavigateToSettings();
        Task NavigateToHome();
        Task SendNotificationsStatus(string status);
        Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse);
        Task SendNotificationUnauthorised();
        Task SendBiometricSpec(BiometricSpec biometricSpec);
        Task SendBiometricCompletion(BiometricCompletion completionDetails);
    }
}
