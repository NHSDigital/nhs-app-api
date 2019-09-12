namespace NHSOnline.Backend.GpSystems.Im1Connection
{
    public interface IIm1ConnectionVerifyResultVisitor<out T>
    {
        T Visit(Im1ConnectionVerifyResult.Success result);

        T Visit(Im1ConnectionVerifyResult.NotFound result);

        T Visit(Im1ConnectionVerifyResult.BadGateway result);

        T Visit(Im1ConnectionVerifyResult.InternalServerError result);

        T Visit(Im1ConnectionVerifyResult.BadRequest result);
        T Visit(Im1ConnectionVerifyResult.Forbidden result);
        T Visit(Im1ConnectionVerifyResult.ErrorCase result);
    }
}