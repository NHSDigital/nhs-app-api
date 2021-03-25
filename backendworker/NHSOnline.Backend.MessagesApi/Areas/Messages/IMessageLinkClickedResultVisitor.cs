using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public interface IMessageLinkClickedResultVisitor<out T>
    {
        T Visit(MessageLinkClickedResult.Success result);
        T Visit(MessageLinkClickedResult.BadRequest result);
        T Visit(MessageLinkClickedResult.BadGateway result);
        T Visit(MessageLinkClickedResult.InternalServerError result);
    }
}
