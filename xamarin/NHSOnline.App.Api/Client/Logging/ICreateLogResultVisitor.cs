
namespace NHSOnline.App.Api.Client.Logging
{
    public interface ICreateLogResultVisitor<T>
    {
        T Visit(ApiCreateLogResult.Created created);
        T Visit(ApiCreateLogResult.Failure failure);
    }
}