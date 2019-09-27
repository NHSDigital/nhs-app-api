using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public interface IMessageResultVisitor<out T>
    {
        T Visit(MessageResult.Success result);
        T Visit(MessageResult.BadGateway result);
        T Visit(MessageResult.InternalServerError result);
    }
}