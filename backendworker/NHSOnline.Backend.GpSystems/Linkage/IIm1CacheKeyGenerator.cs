namespace NHSOnline.Backend.GpSystems.Linkage
{
    public interface IIm1CacheKeyGenerator
    {
        string GenerateCacheKey(string accountId, string odsCode, string linkageKey);
    }
}