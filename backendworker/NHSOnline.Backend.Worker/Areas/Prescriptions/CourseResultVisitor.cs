using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    internal class CourseResultVisitor : ICourseResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetCoursesResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(GetCoursesResult.SupplierSystemUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(GetCoursesResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(GetCoursesResult.SupplierNotEnabled result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
    }
}
