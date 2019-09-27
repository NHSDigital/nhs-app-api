namespace NHSOnline.Backend.Support.Repository
{
    public interface IMongoConfiguration
    {
        string DatabaseName { get; }
        string CollectionName { get; }
        string Host { get; }
        int Port { get; }
        string Username { get; }
        string Password { get; }
    }
}