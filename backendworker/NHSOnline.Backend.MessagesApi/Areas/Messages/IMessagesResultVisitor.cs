using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public interface IMessagesResultVisitor<out T>
    {
        T Visit(MessagesResult.Found result);
        T Visit(MessagesResult.None result);
        T Visit(MessagesResult.BadGateway result);
        T Visit(MessagesResult.BadRequest result);
        T Visit(MessagesResult.InternalServerError result);
    }
}