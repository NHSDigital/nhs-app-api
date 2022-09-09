using System;
using MongoDB.Bson.Serialization.Attributes;

namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public class ReplyOption
    {
        public string Code { get; set; }

        public string Display { get; set; }
    }
}