using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public class AddMessageReply
    {
        public List<AddReplyOption> Options { get; set; } = new List<AddReplyOption>();
    }
}