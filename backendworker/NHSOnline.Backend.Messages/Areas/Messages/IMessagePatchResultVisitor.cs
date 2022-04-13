using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public interface IMessagePatchResultVisitor<out T>
    {
        T Visit(MessagePatchResult.BadRequest result);
        T Visit(MessagePatchResult.NoChange result);
        T Visit(MessagePatchResult.Updated result);
        T Visit(MessagePatchResult.NotFound result);
        T Visit(MessagePatchResult.BadGateway result);
        T Visit(MessagePatchResult.InternalServerError result);
    }
}