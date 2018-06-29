using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Prescriptions
{
    public class EmisCourseService : ICourseService
    {
        private readonly ILogger _logger;
        private readonly ConfigurationSettings _settings;
        private readonly IEmisClient _emisClient;
        private readonly IEmisPrescriptionMapper _emisPrescriptionMapper;

        public EmisCourseService(ILoggerFactory loggerFactory, IOptions<ConfigurationSettings> settings, IEmisClient emisClient, IEmisPrescriptionMapper emisPrescriptionMapper)
        {
            _logger = loggerFactory.CreateLogger<EmisCourseService>();
            _settings = settings.Value;
            _emisClient = emisClient;
            _emisPrescriptionMapper = emisPrescriptionMapper;
        }

        public async Task<GetCoursesResult> GetCourses(UserSession userSession)
        {
            var emisUserSession = (EmisUserSession) userSession;

            try
            {
                var coursesResponse = await _emisClient.CoursesGet(emisUserSession.UserPatientLinkToken, emisUserSession.SessionId, emisUserSession.EndUserSessionId);

                if (coursesResponse.HasSuccessStatusCode)
                {
                    try
                    {
                        _logger.LogDebug("Filtering courses from successful emis response so we are left with only repeat courses which can be requested");
                        coursesResponse.Body.Courses = coursesResponse.Body.Courses
                        .Where(x => x.PrescriptionType == PrescriptionType.Repeat && x.CanBeRequested)
                        .OrderBy(x => x.Name)
                        .Take(_settings.CoursesMaxCoursesLimit.Value);

                        _logger.LogDebug($"Mapping response from {nameof(CoursesGetResponse)} to {nameof(CourseListResponse)}");

                        var courseListResponse = _emisPrescriptionMapper.Map(coursesResponse.Body);

                        return new GetCoursesResult.SuccessfullyRetrieved(courseListResponse);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Something went wrong while building the response.");

                        return new GetCoursesResult.InternalServerError();
                    }
                }

                return GetCorrectErrorResult(coursesResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving courses");
                return new GetCoursesResult.SupplierSystemUnavailable();
            }
        }

        private GetCoursesResult GetCorrectErrorResult(
            EmisClient.EmisApiResponse response)
        {  
            if (response.HasForbiddenResponse())
            {
                _logger.LogError("The emis prescriptions service is not enabled");
                
                return new GetCoursesResult.SupplierNotEnabled();
            }
            
            _logger.LogError("Emis system is currently unavailable");

            return new GetCoursesResult.SupplierSystemUnavailable();       
        }
    }
}
