using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Prescriptions
{
    public class MicrotestCourseService : ICourseService
    {
        private readonly ILogger<MicrotestCourseService> _logger;
        private readonly MicrotestConfigurationSettings _settings;
        private readonly IMicrotestClient _microtestClient;
        private readonly IMicrotestPrescriptionMapper _microtestPrescriptionMapper;

        public MicrotestCourseService(
            ILogger<MicrotestCourseService> logger, 
            MicrotestConfigurationSettings settings, 
            IMicrotestClient emisClient, 
            IMicrotestPrescriptionMapper microtestPrescriptionMapper)
        {
            _logger = logger;
            _settings = settings;
            _microtestClient = emisClient;
            _microtestPrescriptionMapper = microtestPrescriptionMapper;
        }

        public async Task<GetCoursesResult> GetCourses(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var microtestUserSession = (MicrotestUserSession)gpLinkedAccountModel.GpUserSession;

            try
            {
                _logger.LogEnter();
                _logger.LogDebug("Beginning Fetch Courses for user");

                var coursesResponse = await _microtestClient.CoursesGet(microtestUserSession.OdsCode, microtestUserSession.NhsNumber);

                _logger.LogDebug("Fetch Courses for user complete");

                if (coursesResponse.HasSuccessResponse)
                {
                    try
                    {
                        _logger
                            .LogDebug("Filtering courses from successful microtest response so we are left with only repeat courses");

                        var totalCourses = coursesResponse.Body.Courses.Count();

                        coursesResponse.Body.Courses =
                            coursesResponse.Body.Courses
                            .Where(x => string.Equals(x.Status, MedicationStatus.Repeat, StringComparison.OrdinalIgnoreCase))
                            .OrderBy(x => x.Name);

                        var numberOfRepeatCourses = coursesResponse.Body.Courses.Count();

                        if (_settings.CoursesMaxCoursesLimit != null)
                        {
                            coursesResponse.Body.Courses =
                                coursesResponse.Body.Courses
                                    .Take(_settings.CoursesMaxCoursesLimit.Value);
                        }

                        var numberOfCoursesAfterFiltering = coursesResponse.Body.Courses.Count();
                        var numberOfCoursesDiscarded = numberOfRepeatCourses - numberOfCoursesAfterFiltering;
                        
                        var coursesCount = new FilteringCounts
                        {
                            ReceivedCount = totalCourses,
                            FilteredRemainingRepeatsCount = numberOfRepeatCourses,
                            FilteredMaxAllowanceDiscardedCount = numberOfCoursesDiscarded,
                            ReturnedCount = numberOfCoursesAfterFiltering
                        };

                        _logger.LogDebug($"Mapping response from {nameof(CoursesGetResponse)} to {nameof(CourseListResponse)}");

                        var courseListResponse = _microtestPrescriptionMapper.Map(coursesResponse.Body);

                        return new GetCoursesResult.Success(courseListResponse, coursesCount);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Something went wrong while building the response");
                        return new GetCoursesResult.InternalServerError();
                    }
                }               
                else if (coursesResponse.HasForbiddenResponse)
                {
                    _logger.LogError("Microtest prescriptions is not enabled");
                    return new GetCoursesResult.Forbidden();
                }

                return GetCorrectErrorResult(coursesResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving courses");
                return new GetCoursesResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private GetCoursesResult GetCorrectErrorResult(
            MicrotestClient.MicrotestApiResponse response)
        {
            if (response.HasForbiddenResponse)
            {
                _logger.LogError(response.ErrorForLogging);
                return new GetCoursesResult.Forbidden();
            }

            _logger.LogError("Microtest system is currently unavailable");
            _logger.LogError(response.ErrorForLogging);
            return new GetCoursesResult.BadGateway();
        }
    }
}
