using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Prescriptions
{
    public class VisionCourseService : ICourseService
    {
        private readonly ILogger _logger;
        private readonly ConfigurationSettings _settings;
        private readonly IVisionClient _visionClient;
        private readonly IVisionPrescriptionMapper _visionPrescriptionMapper;

        public VisionCourseService(ILogger<VisionCourseService> logger, IOptions<ConfigurationSettings> settings, IVisionClient visionClient, IVisionPrescriptionMapper visionPrescriptionMapper)
        {
            _logger = logger;
            _settings = settings.Value;
            _visionClient = visionClient;
            _visionPrescriptionMapper = visionPrescriptionMapper;
        }

        public async Task<GetCoursesResult> GetCourses(UserSession userSession)
        {
            var visionUserSession = (VisionUserSession) userSession;
            
            try
            {
                _logger.LogDebug("Beginning Fetch Courses for user");
                
                var coursesResponse = await _visionClient.GetEligibleRepeats(visionUserSession);
                
                _logger.LogDebug("Fetch Courses for user complete");

                if (!coursesResponse.HasErrorResponse)
                {
                    try
                    {
                        _logger
                            .LogDebug("Filtering courses from successful vision response. Unfiltered number of courses: " +
                                      $"{coursesResponse.Body.EligibleRepeats.Repeat.Count}");

                        coursesResponse.Body.EligibleRepeats.Repeat = 
                            coursesResponse.Body.EligibleRepeats.Repeat
                            .OrderBy(x => x.Drug).ToList();
                        
                        if (_settings.CoursesMaxCoursesLimit != null)
                        {
                            coursesResponse.Body.EligibleRepeats.Repeat = 
                                coursesResponse.Body.EligibleRepeats.Repeat
                                    .Take(_settings.CoursesMaxCoursesLimit.Value).ToList();
                        }

                        _logger.LogDebug($"Mapping response from {nameof(EligibleRepeatsResponse)} to {nameof(CourseListResponse)}");

                        var courseListResponse = _visionPrescriptionMapper.Map(coursesResponse.Body.EligibleRepeats.Repeat);

                        return new GetCoursesResult.SuccessfullyRetrieved(courseListResponse);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Something went wrong while building the response");

                        return new GetCoursesResult.InternalServerError();
                    }
                }
                
                _logger.LogError($"Vision system encountered an error: { coursesResponse.ErrorContent }");
                
                return new GetCoursesResult.SupplierSystemUnavailable();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving courses");
                return new GetCoursesResult.SupplierSystemUnavailable();
            }
        }
    }
}
