namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal class NotificationRegistrationItem
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