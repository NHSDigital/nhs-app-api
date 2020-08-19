using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    [FakeGpAreaBehaviour(Behaviour.Default)]
    public class DefaultCourseAreaBehaviour : ICourseAreaBehaviour
    {

        private FilteringCounts _filteringCounts = new FilteringCounts
        {
            ReceivedCount = 1,
            ReturnedCount = 0,
            ReceivedRepeatsCount = 1,
            ExcessRepeatsCount = 0,
        };

        private CourseListResponse _fakeCourses = new CourseListResponse
        {
            Courses = new List<Course>
            {
                new Course
                {
                    Id = "FakeCourse1",
                    Name = "FakeMedication1",
                    Details = "Take 1 on days starting with 'T'"
                }
            }
        };

        public async Task<GetCoursesResult> GetCourses(GpLinkedAccountModel gpLinkedAccountModel)
        {
            return await Task.FromResult<GetCoursesResult>(new GetCoursesResult.Success(
                _fakeCourses, _filteringCounts));
        }
    }
}