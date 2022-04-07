using System.Collections.Generic;

namespace NHSOnline.Backend.UserInfo.Areas.UserInfo.Models
{
    public class UserInfoResponse
    {
        public IEnumerable<InfoUser> Users { get; set; } = new List<InfoUser>();
    }
}
