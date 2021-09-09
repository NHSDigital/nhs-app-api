using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public interface IAddMessageResultVisitor<out T>
    {
        T Visit(AddMessageResult.Success result);
        T Visit(AddMessageResult.BadGateway result);
        T Visit(AddMessageResult.BadRequest result);
        T Visit(AddMessageResult.InternalServerError result);
    }
}