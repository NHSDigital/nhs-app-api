using System.Collections.Generic;

namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public class UserSendersResponse
    {
        public List<UserSender> Senders { get; set; } = new List<UserSender>();
    }
}