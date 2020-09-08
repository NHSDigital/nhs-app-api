namespace NHSOnline.Backend.UsersApi.Notifications
{
    public class NotificationRegistrationItem
    {
        public enum RegistrationType
        {
            Installation,
            Registration
        }
        public RegistrationType Type { get; set; }
        public string Id { get; set; }
    }
}