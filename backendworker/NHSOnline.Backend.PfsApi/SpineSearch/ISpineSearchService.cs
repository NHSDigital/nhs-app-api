namespace NHSOnline.Backend.PfsApi.SpineSearch
{
    public interface ISpineSearchService
    {
        NhsAppSpinePdsTraceProperties RetrieveSpinePropertiesForPdsTrace();

        NhsAppSpinePdsUpdateProperties RetrieveSpinePropertiesForPdsUpdate();
    }
}
