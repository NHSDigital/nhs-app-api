namespace NHSOnline.Backend.Support.Repository
{
    public interface IRepositoryFindResultVisitor<TRecord, out T>
    {
        T Visit(RepositoryFindResult<TRecord>.NotFound result);
        T Visit(RepositoryFindResult<TRecord>.Found result);
        T Visit(RepositoryFindResult<TRecord>.InternalServerError result);
        T Visit(RepositoryFindResult<TRecord>.RepositoryError result);
    }
}