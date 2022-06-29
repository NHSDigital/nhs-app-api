using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public interface ISenderPostResultVisitor<out T>
    {
        T Visit(SenderPostResult.Created result);
        T Visit(SenderPostResult.BadGateway result);
        T Visit(SenderPostResult.InternalServerError result);
    }
}