using System.Linq;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Extensions
{
    public static class SessionsPostResponseExtensions
    {
        public static string ExtractUserPatientLinkToken(this SessionsPostResponse sessionsPostResponse)
        {
            return sessionsPostResponse
                ?.UserPatientLinks
                ?.FirstOrDefault(x => x.AssociationType == AssociationType.Self)
                ?.UserPatientLinkToken;
        }
    }
}
