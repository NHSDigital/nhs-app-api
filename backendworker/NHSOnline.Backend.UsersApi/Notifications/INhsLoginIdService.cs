namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INhsLoginIdService
    {
        public bool HandlesNhsLoginId(string nhsLoginId);
    }
}
