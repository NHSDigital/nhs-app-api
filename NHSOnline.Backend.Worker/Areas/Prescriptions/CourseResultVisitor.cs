using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Router.Course;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    internal class CourseResultVisitor : ICourseResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetCoursesResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(GetCoursesResult.Unsuccessful result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}