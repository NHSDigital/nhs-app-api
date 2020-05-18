using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class UserSessionDeleteSteps
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IUserSessionDeleteStep<P5UserSession>> _deleteP5UserSessionSteps;
        private readonly IEnumerable<IUserSessionDeleteStep<P9UserSession>> _deleteP9UserSessionSteps;

        public UserSessionDeleteSteps(
            ILogger<UserSessionDeleteSteps> logger,
            IEnumerable<IUserSessionDeleteStep<UserSession>> deleteUserSessionSteps,
            IEnumerable<IUserSessionDeleteStep<P5UserSession>> deleteP5UserSessionSteps,
            IEnumerable<IUserSessionDeleteStep<P9UserSession>> deleteP9UserSessionSteps)
        {
            _logger = logger;
            _deleteP5UserSessionSteps = deleteUserSessionSteps.Concat(deleteP5UserSessionSteps);
            _deleteP9UserSessionSteps = _deleteP5UserSessionSteps.Concat(deleteP9UserSessionSteps);
        }

        internal async Task<DeleteUserSessionResult> Delete(HttpContext httpContext, P5UserSession userSession)
            => await Delete(httpContext, userSession, _deleteP5UserSessionSteps);

        internal async Task<DeleteUserSessionResult> Delete(HttpContext httpContext, P9UserSession userSession)
            => await Delete(httpContext, userSession, _deleteP9UserSessionSteps);

        private async Task<DeleteUserSessionResult> Delete<TUserSession>(
            HttpContext httpContext,
            TUserSession userSession,
            IEnumerable<IUserSessionDeleteStep<TUserSession>> deleteSteps)
            where TUserSession: UserSession
        {
            var stepTasks = deleteSteps
                .Select(step => BeginExecuteStep(step, httpContext, userSession))
                .ToArray();

            var results = await Task.WhenAll(stepTasks);

            if (results.All(result => result))
            {
                return new DeleteUserSessionResult.Success();
            }
            return new DeleteUserSessionResult.Failure();
        }

        private Task<bool> BeginExecuteStep<TUserSession>(
            IUserSessionDeleteStep<TUserSession> step,
            HttpContext httpContext,
            TUserSession userSession)
            where TUserSession : UserSession
        {
            return Task.Run(async () =>
            {
                try
                {
                    return await step.Delete(httpContext, userSession);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, $"User session delete task {step?.GetType().Name} failed.");
                    return false;
                }
            });
        }
    }
}