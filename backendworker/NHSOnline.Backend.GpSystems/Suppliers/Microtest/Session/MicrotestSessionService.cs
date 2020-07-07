using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Demographics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Session
{
    public class MicrotestSessionService : ISessionService
    {
        private readonly ILogger<MicrotestSessionService> _logger;
        private readonly IMicrotestDemographicsService _demographicsService;
        private readonly MicrotestTokenValidationService _tokenValidationService;

        public MicrotestSessionService(
            ILogger<MicrotestSessionService> logger,
            IMicrotestDemographicsService demographicsService,
            MicrotestTokenValidationService tokenValidationService)
        {
            _logger = logger;
            _demographicsService = demographicsService;
            _tokenValidationService = tokenValidationService;
        }

        public async Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber)
        {
            try
            {
                if (_tokenValidationService.IsInvalidConnectionTokenFormat(connectionToken))
                {
                    _logger.LogError("Invalid Im1 connection token");
                    return new GpSessionCreateResult.InvalidConnectionToken();
                }

                var session = new MicrotestUserSession
                {
                    NhsNumber = nhsNumber,
                    OdsCode = odsCode
                };

                var demographicsResult = await _demographicsService.GetDemographics(new GpLinkedAccountModel(session));

                if (!(demographicsResult is DemographicsResult.Success successfulDemographicsResult))
                {
                    const string message = "Error retrieving demographics when creating session";
                    _logger.LogError(message);
                    return new GpSessionCreateResult.BadGateway(message);
                }

                session.Name = successfulDemographicsResult.Response.PatientName;
                return new GpSessionCreateResult.Success(session);
            }
            catch (NhsTimeoutException ex)
            {
                _logger.LogError(ex.Message);
                return new GpSessionCreateResult.Timeout(ex.Message);
            }
            catch (NhsUnparsableException ex)
            {
                _logger.LogError(ex.Message);
                return new GpSessionCreateResult.Unparseable(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                const string message =
                    "Failed request to create MICROTEST user session, HttpRequestException has been thrown.";
                _logger.LogError(ex, message);
                return new GpSessionCreateResult.BadGateway(message);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession)
        {
            return Task.FromResult((SessionLogoffResult) new SessionLogoffResult.Success(gpUserSession));
        }

        public Task<GpSessionRecreateResult> Recreate(string connectionToken, string odsCode, string nhsNumber, string patientId)
        {
            throw new System.NotImplementedException();
        }
    }
}
