using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
{
    public class FakeSessionService : FakeServiceBase, ISessionService
    {
        private readonly ILogger<FakeSessionService> _logger;

        public FakeSessionService(
            ILogger<FakeSessionService> logger,
            IFakeUserRepository fakeUserRepository)
            : base(logger, fakeUserRepository)
        {
            _logger = logger;
        }

        public async Task<GpSessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber)
        {
            _logger.LogEnter();

            try
            {
                var fakeUser = FindUser(nhsNumber);
                return await fakeUser.SessionBehaviour.Create(connectionToken, odsCode, nhsNumber, fakeUser);
            }
            catch (UnknownFakeUserException)
            {
                return await Task.FromResult<GpSessionCreateResult>(
                    new GpSessionCreateResult.Forbidden("Unknown user"));
            }
            catch (Exception e)
            {
                const string errorMessage = "Something went wrong during building the response.";
                _logger.LogError(e, errorMessage);
                return await Task.FromResult<GpSessionCreateResult>(new GpSessionCreateResult.InternalServerError(errorMessage));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession)
        {
            _logger.LogEnter();

            try
            {
                var fakeUser = FindUser(gpUserSession.NhsNumber);
                return await fakeUser.SessionBehaviour.Logoff(gpUserSession);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during building the response.");
                return await Task.FromResult<SessionLogoffResult>(new SessionLogoffResult.InternalServerError());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public Task<GpSessionRecreateResult> Recreate(string connectionToken, string odsCode, string nhsNumber, string patientId)
        {
            throw new NotImplementedException();
        }
    }
}