using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Prescriptions
{
    public class EmisCourseService : ICourseService
    {
        private readonly ILogger<EmisCourseService> _logger;
        private readonly ConfigurationSettings _settings;
        private readonly IEmisClient _emisClient;
        private readonly IEmisPrescriptionMapper _emisPrescriptionMapper;

        public EmisCourseService(ILogger<EmisCourseService> logger, IOptions<ConfigurationSettings> settings, IEmisClient emisClient, IEmisPrescriptionMapper emisPrescriptionMapper)
        {
            _logger = logger;
            _settings = settings.Value;
            _emisClient = emisClient;
            _emisPrescriptionMapper = emisPrescriptionMapper;
        }

        public async Task<GetCoursesResult> GetCourses(UserSession userSession)
        {
            var emisUserSession = (EmisUserSession) userSession.GpUserSession;
            
            try
            {
                _logger.LogEnter();
                _logger.LogDebug("Beginning Fetch Courses for user");
                
                var coursesResponse = await _emisClient.CoursesGet(emisUserSession.UserPatientLinkToken, emisUserSession.SessionId, 
                    emisUserSession.EndUserSessionId);
                
                _logger.LogDebug("Fetch Courses for user complete");

                if (coursesResponse.HasSuccessResponse)
                {
                    try
                    {
                        _logger
                            .LogDebug("Filtering courses from successful emis response so we are left with only repeat courses which can be requested");

                        coursesResponse.Body.Courses = 
                            coursesResponse.Body.Courses
                            .Where(x => x.PrescriptionType == PrescriptionType.Repeat && x.CanBeRequested)
                            .OrderBy(x => x.Name);
                        
                        if (_settings.CoursesMaxCoursesLimit != null)
                        {
                            coursesResponse.Body.Courses = 
                                coursesResponse.Body.Courses
                                    .Take(_settings.CoursesMaxCoursesLimit.Value);
                        }

                        _logger.LogDebug($"Mapping response from {nameof(CoursesGetResponse)} to {nameof(CourseListResponse)}");
                        
                        var courseListResponse = _emisPrescriptionMapper.Map(coursesResponse.Body);
                        courseListResponse.SpecialRequestNecessity = emisUserSession.PrescriptionSpecialRequestNecessity;

                        return new GetCoursesResult.SuccessfullyRetrieved(courseListResponse);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Something went wrong while building the response");
                        _logger.LogEmisErrorResponse(coursesResponse);
                        return new GetCoursesResult.InternalServerError();
                    }
                }
                
                _logger.LogEmisUnknownError(coursesResponse);
                return GetCorrectErrorResult(coursesResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving courses");
                return new GetCoursesResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private GetCoursesResult GetCorrectErrorResult(
            EmisClient.EmisApiResponse response)
        {  
            if (response.HasForbiddenResponse())
            {
                _logger.LogError("The emis prescriptions service is not enabled");
                _logger.LogEmisErrorResponse(response);
                return new GetCoursesResult.SupplierNotEnabled();
            }
            
            _logger.LogError("Emis system is currently unavailable");
            _logger.LogEmisErrorResponse(response);
            return new GetCoursesResult.SupplierSystemUnavailable();       
        }
    }
}
