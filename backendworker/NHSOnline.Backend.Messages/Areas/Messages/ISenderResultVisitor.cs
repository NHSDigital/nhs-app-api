using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public interface ISenderResultVisitor<out T>
    {
        T Visit(SenderResult.Found result);
        T Visit(SenderResult.NotFound result);
        T Visit(SenderResult.BadGateway result);
        T Visit(SenderResult.InternalServerError result);
    }
}