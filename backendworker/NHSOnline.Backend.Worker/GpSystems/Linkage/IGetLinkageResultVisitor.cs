namespace NHSOnline.Backend.Worker.GpSystems.Linkage
{
    public interface IGetLinkageResultVisitor<out T>
    {
        T Visit(GetLinkageResult.SuccessfullyRetrieved result);

        T Visit(GetLinkageResult.NhsNumberNotFound result);

        T Visit(GetLinkageResult.SupplierSystemUnavailable result);

        T Visit(GetLinkageResult.InternalServerError result);

        T Visit(GetLinkageResult.LinkageKeyRevoked result);
    }
}
