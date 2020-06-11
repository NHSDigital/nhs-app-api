using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Prescriptions
{
    public class VisionCourseService : ICourseService
    {
        private readonly ILogger<VisionCourseService> _logger;
        private readonly VisionConfigurationSettings _settings;
        private readonly IVisionClient _visionClient;
        private readonly IVisionPrescriptionMapper _visionPrescriptionMapper;

        public VisionCourseService(
            ILogger<VisionCourseService> logger,
            VisionConfigurationSettings settings,
            IVisionClient visionClient,
            IVisionPrescriptionMapper visionPrescriptionMapper)
        {
            _logger = logger;
            _settings = settings;
            _visionClient = visionClient;
            _visionPrescriptionMapper = visionPrescriptionMapper;
        }

        public async Task<GetCoursesResult> GetCourses(GpLinkedAccountModel gpLinkedAccountModel)
        {
            _logger.LogEnter();
            var visionUserSession = (VisionUserSession)gpLinkedAccountModel.GpUserSession;

            if (!visionUserSession.IsRepeatPrescriptionsEnabled)
            {
                _logger.LogError("The Vision repeat prescriptions service is not enabled");
                return new GetCoursesResult.Forbidden();
            }

            try
            {
                _logger.LogDebug("Beginning Fetch Courses for user");

                var coursesResponse = await _visionClient.GetEligibleRepeats(visionUserSession);

                _logger.LogDebug("Fetch Courses for user complete");

                if (!coursesResponse.HasErrorResponse)
                {
                    try
                    {
                        var totalCourses = coursesResponse.Body.EligibleRepeats.Repeats.Count;

                        _logger
                            .LogDebug("Filtering courses from successful vision response. Unfiltered number of courses: " +
                                      $"{coursesResponse.Body.EligibleRepeats.Repeats.Count}");

                        coursesResponse.Body.EligibleRepeats.Repeats =
                            coursesResponse.Body.EligibleRepeats.Repeats
                            .OrderBy(x => x.Drug).ToList();

                        if (_settings.CoursesMaxCoursesLimit.HasValue)
                        {
                            coursesResponse.Body.EligibleRepeats.Repeats =
                                coursesResponse.Body.EligibleRepeats.Repeats
                                .Take(_settings.CoursesMaxCoursesLimit.Value).ToList();
                        }

                        var numberOfCoursesAfterFiltering = coursesResponse.Body.EligibleRepeats.Repeats.Count;
                        var numberOfCoursesDiscarded = totalCourses - numberOfCoursesAfterFiltering;

                        var coursesCount = new FilteringCounts
                        {
                            ReceivedCount = totalCourses,
                            ReceivedRepeatsCount = totalCourses,
                            ExcessRepeatsCount = numberOfCoursesDiscarded,
                            ReturnedCount = numberOfCoursesAfterFiltering
                        };

                        _logger.LogDebug($"Mapping response from {nameof(EligibleRepeatsResponse)} to {nameof(CourseListResponse)}");

                        var courseListResponse = _visionPrescriptionMapper.Map(coursesResponse.Body.EligibleRepeats);

                        visionUserSession.AllowFreeTextPrescriptions = coursesResponse.Body.EligibleRepeats.Settings.AllowFreeText;

                        return new GetCoursesResult.Success(courseListResponse,
                            coursesCount,
                            visionUserSession.AllowFreeTextPrescriptions);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Something went wrong while building the response");
                        return new GetCoursesResult.InternalServerError();
                    }
                }

                _logger.LogError($"Vision system encountered an error: { coursesResponse.ErrorForLogging }");
                _logger.LogVisionErrorResponse(coursesResponse);

                return new GetCoursesResult.BadGateway();
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
    }
}
