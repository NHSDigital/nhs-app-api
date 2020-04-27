namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public interface IMongoSessionCacheServiceConfig
    {
        string MongoDatabaseName { get; }
        string MongoDatabaseSessionCollectionName { get; }
    }
}