namespace NHSOnline.Backend.GpSystems.SessionManager
{
    internal interface IMongoSessionCacheServiceConfig
    {
        string DatabaseName { get; }
        string CollectionName { get; }
        string SecondaryDatabaseName { get; }
        string SecondaryCollectionName { get; }
    }
}