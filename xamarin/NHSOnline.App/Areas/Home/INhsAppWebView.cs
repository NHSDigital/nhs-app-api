using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.App.Controls;
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
            Action<WebNavigatingEventArgs>? Navigating { get; set; }
            Func<Uri, Task>? NavigationFailed { get; set; }

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
            Func<OpenPostWebIntegrationRequest, Task>? OpenPostWebIntegrationRequested { get; set; }
            Func<AddEventToCalendarRequest, Task>? AddEventToCalendarRequested { get; set; }

            Func<DownloadRequest, Task>? StartDownloadRequested { get; set; }
            Func<StartNhsLoginUpliftRequest, Task>? StartNhsLoginUpliftRequested { get; set; }

            Func<Task>? GetNotificationsStatusRequested { get; set; }
            Func<string, Task>? GetPnsTokenRequested { get; set; }

            Func<string, Task>? FetchBiometricStatusRequested { get; set; }
            Func<Task>? FetchNativeAppVersionRequested { get; set; }
            Func<string, Task>? SetMenuBarItemRequested { get; set; }
            Func<Task>? ClearMenuBarItemRequested { get; set; }
            Func<string, Task>? UpdateBiometricRegistrationRequested { get; set; }
            Func<Uri, Task>? OpenBrowserOverlayRequested { get; set; }
            Func<Task>? OpenSettingsRequested { get; set; }
            Func<Task>? LogoutRequested { get; set; }
            Func<Task>? SessionExpiredRequested { get; set; }
            Func<Task>? BackRequested { get; set; }
            Func<CreateOnDemandGpSessionRequest, Task>? CreateOnDemandGpSessionRequested { get; set; }
            Func<Uri, Task>? DeeplinkRequested { get; set; }
            Func<Task>? DisplayPageLeaveWarningRequested { get; set; }
            Func<Task>? OnSessionExpiringRequested { get; set; }
        }

        NavigationFooterItem SelectedNavigationFooterItem { get; set; }

        void GoToUri(Uri uri);
        Task NavigateToAppPage(string page);
        Task NavigateToAdvice();
        Task NavigateToAppointments();
        Task NavigateToPrescriptions();
        Task NavigateToYourHealth();
        Task NavigateToMessages();
        Task NavigateToMore();
        Task NavigateToHome();
        Task NavigateToRedirector(Uri targetUrl);
        Task NavigateToOnDemandGpReturn(Dictionary<string, string> queryParameters);
        Task SendNotificationsStatus(string status);
        Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse);
        Task SendNotificationUnauthorised();
        Task SendBiometricStatus(BiometricStatus biometricStatus);
        Task SendBiometricCompletion(BiometricCompletion completionDetails);
        Task SendSessionExtend();
        Task SendStayOnPage();
        Task SendLeavePage();
        Task Logout();
        Task GetContextualHelpLink();
        Task UpdateNativeVersion(string version);

        View GetWebViewElement();
    }
}
