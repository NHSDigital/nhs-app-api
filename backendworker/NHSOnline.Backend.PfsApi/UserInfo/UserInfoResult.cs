using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    [SuppressMessage("Microsoft.Design", "CA1052", Justification = "No need for a visitor in this instance" )]
    public abstract class UserInfoResult
    {
        public class Success : UserInfoResult
        {
        }

        public class BadGateway : UserInfoResult
        {
        }

        public class InternalServerError : UserInfoResult
        {
        }
    }
}