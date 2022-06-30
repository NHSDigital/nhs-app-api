using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Cache.Messages
{
    public interface ISenderCacheProvider
    {
        void SetSender(Sender sender);
        Sender GetSender(string senderId);
    }
}