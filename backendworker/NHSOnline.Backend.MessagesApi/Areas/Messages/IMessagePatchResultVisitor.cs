using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public interface IMessagePatchResultVisitor<out T>
    {
        T Visit(MessagePatchResult.BadRequest result);
        T Visit(MessagePatchResult.Updated result);
        T Visit(MessagePatchResult.NotFound result);
        T Visit(MessagePatchResult.BadGateway result);
        T Visit(MessagePatchResult.InternalServerError result);
    }
}