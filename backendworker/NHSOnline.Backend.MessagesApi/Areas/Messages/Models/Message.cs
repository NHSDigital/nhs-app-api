using System;
using MongoDB.Bson;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public class Message
    {
        public ObjectId Id { get; set; }

        public string Sender { get; set; }
        
        public int Version { get; set; }

        public string Body { get; set; }

        public bool Read { get; set; }

        public DateTime SentTime { get; set; }
    }
}