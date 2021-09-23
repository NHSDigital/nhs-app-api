using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public interface ISendersResultVisitor<out T>
    {
        T Visit(SendersResult.Found result);
        T Visit(SendersResult.None result);
        T Visit(SendersResult.BadGateway result);
        T Visit(SendersResult.InternalServerError result);
    }
}