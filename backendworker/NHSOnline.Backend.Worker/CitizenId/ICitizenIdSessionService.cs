using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public interface ICitizenIdSessionService
    {
        [SuppressMessage("Microsoft.Design", "CA1054", Justification = "Uris are not serializable")]
        Task<CitizenIdSessionResult> Create(string authCode, string codeVerifier, string redirectUrl);
    }
}