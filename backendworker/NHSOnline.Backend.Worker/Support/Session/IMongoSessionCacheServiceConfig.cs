namespace NHSOnline.Backend.Worker.Support.Session
{
    public interface IMongoSessionCacheServiceConfig
    {
        string SessionMongoDatabaseHost { get; }
        int SessionMongoDatabasePort { get; }  
        string SessionMongoDatabaseUsername { get; }
        string SessionMongoDatabasePassword { get; }
            
        string SessionMongoDatabaseName { get; }
        string SessionMongoDatabaseCollection { get; }
    }
}