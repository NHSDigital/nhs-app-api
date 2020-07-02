using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    public class FakeCourseService : FakeServiceBase, ICourseService
    {
        private readonly ILogger<FakeCourseService> _logger;

        public FakeCourseService(
            ILogger<FakeCourseService> logger,
            IFakeUserRepository fakeUserRepository)
            : base(logger, fakeUserRepository)
        {
            _logger = logger;
        }

        public async Task<GetCoursesResult> GetCourses(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();

            try
            {
                var fakeUser = await FindUser(gpLinkedAccountModel);
                return await fakeUser.CourseAreaBehaviour.GetCourses(gpLinkedAccountModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during building the response.");
                return await Task.FromResult<GetCoursesResult>(new GetCoursesResult.InternalServerError());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}