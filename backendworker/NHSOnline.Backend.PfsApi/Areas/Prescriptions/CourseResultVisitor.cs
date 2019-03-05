using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    internal class CourseResultVisitor : ICourseResultVisitor<Task<IActionResult>>
    {
        readonly ISessionCacheService _sessionCacheService;
        readonly UserSession _userSession;

        public CourseResultVisitor(ISessionCacheService sessionCacheService, UserSession userSession)
        {
            _sessionCacheService = sessionCacheService;
            _userSession = userSession;
        }

        public async Task<IActionResult> Visit(GetCoursesResult.SuccessfullyRetrieved result)
        {
            if (result.AllowFreeTextPrescriptions != null)
            {
                await _sessionCacheService.UpdateUserSession(_userSession);
            }

            return new OkObjectResult(result.Response);
        }

        public async Task<IActionResult> Visit(GetCoursesResult.SupplierSystemUnavailable result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status502BadGateway));
        }
        
        public async Task<IActionResult> Visit(GetCoursesResult.InternalServerError result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status500InternalServerError));
        }
        
        public async Task<IActionResult> Visit(GetCoursesResult.SupplierNotEnabled result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status403Forbidden));
        }
    }
}
