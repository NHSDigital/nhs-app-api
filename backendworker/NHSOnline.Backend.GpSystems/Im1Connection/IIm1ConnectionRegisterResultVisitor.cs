namespace NHSOnline.Backend.GpSystems.Im1Connection
{
    public interface IIm1ConnectionRegisterResultVisitor<out T>
    {
        T Visit(Im1ConnectionRegisterResult.Success result);
        T Visit(Im1ConnectionRegisterResult.BadRequest result);
        T Visit(Im1ConnectionRegisterResult.NotFound result);
        T Visit(Im1ConnectionRegisterResult.AccountAlreadyExists result);
        T Visit(Im1ConnectionRegisterResult.BadGateway result);
    }
}