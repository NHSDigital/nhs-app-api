using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    internal class CourseResultVisitor : ResultVisitorBase, ICourseResultVisitor<Task<IActionResult>>
    {
        private readonly ISessionCacheService _sessionCacheService;

        public CourseResultVisitor(
            ISessionCacheService sessionCacheService,
            IErrorReferenceGenerator errorReferenceGenerator,
            UserSession userSession) :
            base(errorReferenceGenerator, userSession)
        {
            _sessionCacheService = sessionCacheService;
        }

        protected override ErrorCategory ErrorCategory => ErrorCategory.Prescriptions;

        public async Task<IActionResult> Visit(GetCoursesResult.Success result)
        {
            if (result.AllowFreeTextPrescriptions != null)
            {
                await _sessionCacheService.UpdateUserSession(UserSession);
            }

            return new OkObjectResult(result.Response);
        }

        public async Task<IActionResult> Visit(GetCoursesResult.BadGateway result)
        {
            return await Task.FromResult(BuildErrorResult(StatusCodes.Status502BadGateway));
        }

        public async Task<IActionResult> Visit(GetCoursesResult.InternalServerError result)
        {
            return await Task.FromResult(BuildErrorResult(StatusCodes.Status500InternalServerError));
        }

        public async Task<IActionResult> Visit(GetCoursesResult.Forbidden result)
        {
            return await Task.FromResult(BuildErrorResult(StatusCodes.Status403Forbidden));
        }
    }
}
