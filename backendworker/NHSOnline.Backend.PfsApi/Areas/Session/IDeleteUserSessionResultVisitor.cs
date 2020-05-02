using NHSOnline.Backend.PfsApi.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    internal interface IDeleteUserSessionResultVisitor<out T>
    {
        T Visit(DeleteUserSessionResult.Success success);
        T Visit(DeleteUserSessionResult.Failure failure);
    }
}