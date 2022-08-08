using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public interface IMessagesMetadataResultVisitor<out T>
    {
        T Visit(MessagesMetadataResult.Found result);
        T Visit(MessagesMetadataResult.BadGateway result);
        T Visit(MessagesMetadataResult.InternalServerError result);
    }
}