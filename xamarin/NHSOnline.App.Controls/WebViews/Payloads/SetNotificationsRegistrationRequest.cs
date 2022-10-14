namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public class SetNotificationsRegistrationRequest : NotificationsRegistration
    {
        public string NhsLoginId { get; set; } = string.Empty;
    }
}