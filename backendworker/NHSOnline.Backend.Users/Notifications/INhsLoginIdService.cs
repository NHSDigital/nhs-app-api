namespace NHSOnline.Backend.Users.Notifications
{
    public interface INhsLoginIdService
    {
        public bool HandlesNhsLoginId(string nhsLoginId);
    }
}
