namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Client
{
    internal interface IEmisRequestBuilderMethod
    {
        IEmisRequestBuilderContent Post(string uri);

        IEmisRequestBuilderContent Put(string uri);

        IEmisRequestBuilder Get(string uriFormat, params object[] args);

        IEmisRequestBuilderContent Delete(string uri);
        IEmisRequestBuilderContent Delete(string uriFormat, params object[] args);
    }
}