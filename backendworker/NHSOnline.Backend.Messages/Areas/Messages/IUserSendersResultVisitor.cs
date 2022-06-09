using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public interface IUserSendersResultVisitor<out T>
    {
        T Visit(UserSendersResult.Found result);
        T Visit(UserSendersResult.None result);
        T Visit(UserSendersResult.BadGateway result);
        T Visit(UserSendersResult.InternalServerError result);
    }
}