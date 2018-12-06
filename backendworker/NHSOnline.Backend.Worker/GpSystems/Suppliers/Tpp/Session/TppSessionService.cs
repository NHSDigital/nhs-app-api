using System;
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
        private readonly ITppSessionMapper _sessionMapper;

        public TppSessionService(ITppClient client,
            ILogger<TppSessionService> logger,
            ITppSessionMapper sessionMapper)
        {
            _client = client;
            _logger = logger;
            _sessionMapper = sessionMapper;
        }

        public async Task<SessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber,
            string accessToken)
        {
            try
            {
                _logger.LogEnter(nameof(Create));
                _logger.LogDebug($"Creating using ODS code: {odsCode}");

                var tppToken = connectionToken.DeserializeJson<TppConnectionToken>();
                var authenticate = new Authenticate
                {
                    AccountId = tppToken.AccountId,
                    Passphrase = tppToken.Passphrase,
                    ProviderId = tppToken.ProviderId,
                    UnitId = odsCode
                };

                var reply = await _client.AuthenticatePost(authenticate);

                if (!reply.HasSuccessResponse)
                {
                    _logger.LogError("Failed to authenticate user for TPP");
                    return new SessionCreateResult.SupplierSystemUnavailable();
                }
                
                var userSession = _sessionMapper.Map(reply, odsCode, accessToken, nhsNumber);
                if (!userSession.HasValue)
                {
                    _logger.LogError("Cannot create a valid session from Tpp response");
                    return new SessionCreateResult.SupplierSystemBadResponse();
                }

                _logger.LogDebug($"TPP user session successfully create to OdsCode {odsCode}");
                return new SessionCreateResult.SuccessfullyCreated(
                    reply.Body.User?.Person?.PersonName?.Name,
                    userSession.ValueOrFailure());
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

        public async Task<SessionLogoffResult> Logoff(UserSession userSession)
        {
            try
            {
                _logger.LogEnter(nameof(Logoff));
            
                var tppUserSession = (TppUserSession) userSession;
                var logoffReply = await _client.LogoffPost(tppUserSession);

                if (logoffReply.NotAuthenticated)
                {
                    _logger.LogWarning("User does not have a valid session");
                    return new SessionLogoffResult.NotAuthenticated();
                }

                if (!logoffReply.HasSuccessResponse) 
                {
                    return new SessionLogoffResult.SupplierSystemUnavailable();
                }

                _logger.LogDebug($"TPP user session successfully deleted");
                return new SessionLogoffResult.SuccessfullyDeleted(userSession);

            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Failed request to logoff TPP user session, HttpRequestException has been thrown.");
                return new SessionLogoffResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit(nameof(Logoff));
            }

        }
    }
}
