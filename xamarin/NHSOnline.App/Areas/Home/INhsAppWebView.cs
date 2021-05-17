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

            Func<Task>? HelpRequested { get; set; }
            Func<Task>? HomeRequested { get; set; }
            Func<Task>? AdviceRequested { get; set; }
            Func<Task>? AppointmentsRequested { get; set; }
            Func<Task>? PrescriptionsRequested { get; set; }
            Func<Task>? YourHealthRequested { get; set; }
            Func<Task>? MoreRequested { get; set; }
            Func<Task>? MessagesRequested { get; set; }

            Func<Task>? ResetAndShowErrorRequested { get; set; }

            Func<OpenWebIntegrationRequest, Task>? OpenWebIntegrationRequested { get; set; }
            Func<StartNhsLoginUpliftRequest, Task>? StartNhsLoginUpliftRequested { get; set; }

            Func<Task>? GetNotificationsStatusRequested { get; set; }
            Func<string, Task>? GetPnsTokenRequested { get; set; }

            Func<string, Task>? FetchBiometricStatusRequested { get; set; }
            Func<string, Task>? SetMenuBarItemRequested { get; set; }
            Func<Task>? ClearMenuBarItemRequested { get; set; }
            Func<string, Task>? UpdateBiometricRegistrationRequested { get; set; }
            Func<Task>? OpenSettingsRequested { get; set; }
            Func<Task>? LogoutRequested { get; set; }
            Func<Uri, Task>? DeeplinkRequested { get; set; }
        }

        void GoToUri(Uri uri);
        Task NavigateToRedirectedPathWithinApp(string spaPath);
        Task NavigateToAdvice();
        Task NavigateToAppointments();
        Task NavigateToPrescriptions();
        Task NavigateToYourHealth();
        Task NavigateToMessages();
        Task NavigateToMore();
        Task NavigateToHome();
        Task NavigateToRedirector(Uri targetUrl);
        Task SendNotificationsStatus(string status);
        Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse);
        Task SendNotificationUnauthorised();
        Task SendBiometricStatus(BiometricStatus biometricStatus);
        Task SendBiometricCompletion(BiometricCompletion completionDetails);
    }
}
