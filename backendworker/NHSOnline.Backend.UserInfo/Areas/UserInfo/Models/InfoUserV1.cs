using System;

namespace NHSOnline.Backend.UserInfo.Areas.UserInfo.Models
{
    public class InfoUserV1
    {
        public string NhsLoginId { get; set; }
        public InfoV1 Info { get; set; }
        public DateTime Timestamp { get; set; }
    }
}