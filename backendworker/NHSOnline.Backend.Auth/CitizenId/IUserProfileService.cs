using System.Runtime.CompilerServices;
using NHSOnline.Backend.Auth.CitizenId.Models;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public interface IUserProfileService
    {
        UserProfile GetExistingUserProfileOrThrow([CallerMemberName] string context = "");
    }
}