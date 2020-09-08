using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.UsersApi.Notifications.Models
{
    public class Notification
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }
        public Uri Url { get; set; }

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
    }
}