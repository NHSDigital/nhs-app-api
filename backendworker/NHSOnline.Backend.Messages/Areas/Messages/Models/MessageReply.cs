using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public class MessageReply
    {
        public List<ReplyOption> Options { get; set; } = new List<ReplyOption>();

        public string Response { get; set; }

        public DateTime? ResponseSentDateTime { get; set; }

        public string Status { get; set; }

        public DateTime? ResponseCompletedDateTime { get; set; }
    }
}