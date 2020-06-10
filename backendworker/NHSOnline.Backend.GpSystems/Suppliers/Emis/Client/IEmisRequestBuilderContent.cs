namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Client
{
    internal interface IEmisRequestBuilderContent
    {
        IEmisRequestBuilder Request<TRequest>(TRequest body);
        IEmisRequestBuilder EmptyBody();
    }
}