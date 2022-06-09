using System.Threading.Tasks;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public interface ISenderService
    {
        Task<SenderResult> GetSender(string senderId);
        Task<SenderPostResult> Create(Sender sender);
    }
}