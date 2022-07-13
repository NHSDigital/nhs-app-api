using System;

namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public class Message
    {
        public string Id { get; set; }

        public string SenderId { get; set; }

        public string Sender { get; set; }

        public int Version { get; set; }

        public string Body { get; set; }

        public bool Read { get; set; }

        public DateTime SentTime { get; set; }
    }
}