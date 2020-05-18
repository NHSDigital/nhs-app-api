namespace NHSOnline.Backend.Support.Repository
{
    public interface IRepositoryUpdateResultVisitor<TRecord, out T>
    {
        T Visit(RepositoryUpdateResult<TRecord>.Updated result);
        T Visit(RepositoryUpdateResult<TRecord>.NotFound result);
        T Visit(RepositoryUpdateResult<TRecord>.InternalServerError result);
        T Visit(RepositoryUpdateResult<TRecord>.RepositoryError result);
    }
}