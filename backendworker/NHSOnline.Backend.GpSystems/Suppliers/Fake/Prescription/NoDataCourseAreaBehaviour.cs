using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    [FakeGpAreaBehaviour(Behaviour.NoData)]
    public class NoDataCourseBehaviour : ICourseAreaBehaviour
    {

        private readonly FilteringCounts _filteringCounts = new FilteringCounts
        {
            ReceivedCount = 0,
            ReturnedCount = 0,
            ReceivedRepeatsCount = 0,
            ExcessRepeatsCount = 0,
        };

        private readonly CourseListResponse _fakeCourses = new CourseListResponse
        {
            Courses = new List<Course> ()
        };

        public async Task<GetCoursesResult> GetCourses(GpLinkedAccountModel gpLinkedAccountModel)
        {
            return await Task.FromResult<GetCoursesResult>(new GetCoursesResult.Success(
                _fakeCourses, _filteringCounts));
        }
    }
}