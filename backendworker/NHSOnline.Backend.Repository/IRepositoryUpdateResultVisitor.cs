namespace NHSOnline.Backend.Repository
{
    public interface IRepositoryUpdateResultVisitor<TRecord, out T>
    {
        T Visit(RepositoryUpdateResult<TRecord>.Updated result);
        T Visit(RepositoryUpdateResult<TRecord>.NotFound result);
        T Visit(RepositoryUpdateResult<TRecord>.RepositoryError result);
        T Visit(RepositoryUpdateResult<TRecord>.NoChange result);
    }
}