namespace NHSOnline.Backend.GpSystems.Im1Connection
{
    public interface IIm1ConnectionRegisterResultVisitor<out T>
    {
        T Visit(Im1ConnectionRegisterResult.Success result);
        T Visit(Im1ConnectionRegisterResult.BadGateway result);
        T Visit(Im1ConnectionRegisterResult.UnmappedErrorWithStatusCode result);
        T Visit(Im1ConnectionRegisterResult.ErrorCase result);
    }
}