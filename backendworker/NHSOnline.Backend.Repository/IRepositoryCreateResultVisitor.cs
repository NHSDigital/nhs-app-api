namespace NHSOnline.Backend.Repository
{
    public interface IRepositoryCreateResultVisitor<TRecord, out T>
    {
        T Visit(RepositoryCreateResult<TRecord>.Created result);
        T Visit(RepositoryCreateResult<TRecord>.InternalServerError result);
        T Visit(RepositoryCreateResult<TRecord>.RepositoryError result);
    }
}