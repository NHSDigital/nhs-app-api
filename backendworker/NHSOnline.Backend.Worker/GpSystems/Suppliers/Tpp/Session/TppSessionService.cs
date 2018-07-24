using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Session
{
    public class TppSessionService : ISessionService
    {
        private readonly ITppClient _client;
        private readonly ILogger<TppSessionService> _logger;

        public TppSessionService(ITppClient client,
            ILogger<TppSessionService> logger)
        {
            _client = client;
            _logger = logger;
        }
        
        public async Task<SessionCreateResult> Create(string im1ConnectionToken, string odsCode)
        {
            try
            {
                _logger.LogEnter(nameof(Create));
                _logger.LogDebug($"Creating using ODS code: {odsCode}");

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
                    _logger.LogError("Failed to authenticate user for TPP");
                    return new SessionCreateResult.SupplierSystemUnavailable();
                }

                var suidHeader = reply.Headers?.FirstOrDefault(h => h.Key == "suid");
                var userSession = new TppUserSession
                {
                    Suid = suidHeader?.Value,
                    OnlineUserId = reply.Body.OnlineUserId,
                    PatientId = reply.Body.PatientId,
                    UnitId = odsCode,
                    NhsNumber = reply?.Body?.User?.Person?.NationalId.Value
                };

                _logger.LogDebug($"TPP user session successfully create to OdsCode {odsCode}");
                return new SessionCreateResult.SuccessfullyCreated(
                    reply.Body.User?.Person?.PersonName?.Name,
                    userSession);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Failed request to create TPP user session, HttpRequestException has been thrown.");
                return new SessionCreateResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit(nameof(Create));
            }
        }
    }
}