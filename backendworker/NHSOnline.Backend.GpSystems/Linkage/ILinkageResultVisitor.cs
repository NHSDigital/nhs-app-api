namespace NHSOnline.Backend.GpSystems.Linkage
{
    public interface ILinkageResultVisitor<out T>
    {
        T Visit(LinkageResult.ErrorCase result);
        T Visit(LinkageResult.SupplierSystemUnavailable result);
        T Visit(LinkageResult.InternalServerError result);
        T Visit(LinkageResult.SuccessfullyRetrieved result);
        T Visit(LinkageResult.SuccessfullyCreated result);
        T Visit(LinkageResult.SuccessfullyRetrievedAlreadyExists result);
        T Visit(LinkageResult.NotFound result);
        T Visit(LinkageResult.BadRequest result);
        T Visit(LinkageResult.Conflict result);
        T Visit(LinkageResult.Forbidden result);
        T Visit(LinkageResult.UnknownError result);
    }
}
