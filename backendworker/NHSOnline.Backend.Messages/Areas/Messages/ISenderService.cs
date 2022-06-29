using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public interface ISenderService
    {
        Task<SenderPostResult> Create(Sender sender);
        Task<SendersResult> GetSender(string senderId);
        Task<SendersResult> GetSenders(DateTime lastUpdatedBefore, int limit);
    }
}