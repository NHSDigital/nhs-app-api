using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.UsersApi.Notifications.Models
{
    public class NotificationRequest
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }
        public Uri Url { get; set; }
        public string NhsLoginId { get; set; }

        public IDictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                { "title", Title },
                { "subtitle", Subtitle },
                { "body", Body },
                { "url", Url?.ToString() }
            };
        }

        public override string ToString() => string.Join(",", ToDictionary().Select(x => $"[{x.Key}:{x.Value}]"));
    }
}