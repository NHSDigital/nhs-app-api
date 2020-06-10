using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Client
{
    internal interface IEmisRequestBuilderType
    {
        IEmisRequestBuilderMethod RequestType(RequestsForSuccessOutcome requestType);
    }
}