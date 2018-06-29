namespace NHSOnline.Backend.Worker.GpSystems.Linkage
{
    public interface ICreateLinkageResultVisitor<out T>
    {
        T Visit(CreateLinkageResult.SuccessfullyRetrieved result);

        T Visit(CreateLinkageResult.NhsNumberNotFound result);

        T Visit(CreateLinkageResult.LinkageKeyAlreadyExists result);

        T Visit(CreateLinkageResult.SupplierSystemUnavailable result);

        T Visit(CreateLinkageResult.InternalServerError result);
    }
}
