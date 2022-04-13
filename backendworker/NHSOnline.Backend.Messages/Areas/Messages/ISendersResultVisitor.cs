using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public interface ISendersResultVisitor<out T>
    {
        T Visit(SendersResult.Found result);
        T Visit(SendersResult.None result);
        T Visit(SendersResult.BadGateway result);
        T Visit(SendersResult.InternalServerError result);
    }
}