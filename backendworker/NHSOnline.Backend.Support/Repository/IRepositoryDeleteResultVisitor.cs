namespace NHSOnline.Backend.Support.Repository
{
    public interface IRepositoryDeleteResultVisitor<TRecord, out T>
    {
        T Visit(RepositoryDeleteResult<TRecord>.Deleted result);
        T Visit(RepositoryDeleteResult<TRecord>.NotFound result);
        T Visit(RepositoryDeleteResult<TRecord>.InternalServerError result);
        T Visit(RepositoryDeleteResult<TRecord>.RepositoryError result);
    }
}