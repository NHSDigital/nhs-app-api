using System.Collections.Generic;

namespace NHSOnline.Backend.UserInfo.Areas.UserInfo.Models
{
    public class UserInfoResponseV2
    {
        public IEnumerable<InfoUserV2> Users { get; set; } = new List<InfoUserV2>();
    }
}
