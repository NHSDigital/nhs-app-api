using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public interface ICanonicalSenderNameService
    {
        Task UpdateWithCanonicalSenderName(ICollection<UserMessage> messages);
    }
}