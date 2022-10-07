using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public interface IUnreadMessageCountResultVisitor<out T>
    {
        T Visit(UnreadMessageCountResult.Success result);
        T Visit(UnreadMessageCountResult.Failure result);
    }
}