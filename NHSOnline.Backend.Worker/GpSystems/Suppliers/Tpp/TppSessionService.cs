using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppSessionService: ISessionService
    {
        private readonly ITppClient _client;
        private readonly ConfigurationSettings _settings;

        public TppSessionService(ITppClient client, IOptions<ConfigurationSettings> settings)
        {
            _client = client;
            _settings = settings.Value;
        }
        
        public async Task<SessionCreateResult> Create(string im1ConnectionToken, string odsCode)
        {
            var tppToken = im1ConnectionToken.DeserializeJson<TppConnectionToken>();
            var authenticate = new Authenticate
            {
                AccountId = tppToken.AccountId,
                Passphrase = tppToken.Passphrase,
                UnitId = odsCode
            };

            var reply = await _client.AuthenticatePost(authenticate);
            var userSession = new TppUserSession
            {
                SessionId = reply.Suid
            };
            
            return new SessionCreateResult.SuccessfullyCreated(
                reply.User?.Person?.PersonName?.Name, 
                userSession, 
                _settings.DefaultSessionExpiryMinutes);
        }
    }
}