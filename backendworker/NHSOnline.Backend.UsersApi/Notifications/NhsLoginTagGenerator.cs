
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal static class NhsLoginTagGenerator
    {
        public static string Generate(string nhsLoginId) =>
            $"{Constants.UsersConstants.NhsLoginIdTagPrefix}{Constants.UsersConstants.TagSeparator}{nhsLoginId}";
    }
}