using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Session
{
    public class TppSessionService : ISessionService
    {
        private readonly ITppClient _client;

        public TppSessionService(ITppClient client)
        {
            _client = client;
        }
        
        public async Task<SessionCreateResult> Create(string im1ConnectionToken, string odsCode)
        {
            try
            {
                var tppToken = im1ConnectionToken.DeserializeJson<TppConnectionToken>();
                var authenticate = new Authenticate
                {
                    AccountId = tppToken.AccountId,
                    Passphrase = tppToken.Passphrase,
                    UnitId = odsCode
                };

                var reply = await _client.AuthenticatePost(authenticate);

                if (!reply.HasSuccessResponse)
                {
                    return new SessionCreateResult.SupplierSystemUnavailable();
                }

                var suidHeader = reply.Headers?.FirstOrDefault(h => h.Key == "suid");
                var userSession = new TppUserSession
                {
                    Suid = suidHeader?.Value,
                    OnlineUserId = reply.Body.OnlineUserId,
                    PatientId = reply.Body.PatientId,
                    UnitId = odsCode
                };

                return new SessionCreateResult.SuccessfullyCreated(
                    reply.Body.User?.Person?.PersonName?.Name,
                    userSession);
            }
            catch (HttpRequestException)
            {
                return new SessionCreateResult.SupplierSystemUnavailable();
            }
        }
    }
}