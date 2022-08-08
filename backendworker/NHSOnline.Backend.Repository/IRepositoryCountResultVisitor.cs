namespace NHSOnline.Backend.Repository
{
    public interface IRepositoryCountResultVisitor<out T>
    {
        T Visit(RepositoryCountResult.Found result);
        T Visit(RepositoryCountResult.RepositoryError result);
    }
}