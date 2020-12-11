namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    public interface IIm1CacheServiceConfig
    {
        string MongoDatabaseName { get; }
        string MongoDatabaseIm1CollectionName { get; }
    }
}