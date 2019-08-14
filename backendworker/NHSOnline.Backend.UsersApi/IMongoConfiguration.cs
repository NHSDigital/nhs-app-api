namespace NHSOnline.Backend.UsersApi
{
    internal interface IMongoConfiguration
    {
        string DatabaseName { get; }
        string UserDeviceCollectionName { get; }
        string Host { get; }
        int Port { get; }
        string Username { get; }
        string Password { get; }
    }
}