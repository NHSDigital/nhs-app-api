using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions
{
    public class EmisCourseService : ICourseService
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<EmisCourseService> _logger;
        private readonly EmisConfigurationSettings _settings;
        private readonly IEmisClient _emisClient;
        private readonly IEmisPrescriptionMapper _emisPrescriptionMapper;
        private const string AuditType = AuditingOperations.RepeatPrescriptionsViewRepeatMedicationsResponse;

        public EmisCourseService(IAuditor auditor, ILogger<EmisCourseService> logger, EmisConfigurationSettings settings, IEmisClient emisClient, IEmisPrescriptionMapper emisPrescriptionMapper)
        {
            _logger = logger;
            _settings = settings;
            _emisClient = emisClient;
            _emisPrescriptionMapper = emisPrescriptionMapper;
            _auditor = auditor;

            _settings.Validate();
        }

        public async Task<GetCoursesResult> GetCourses(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var emisUserSession = (EmisUserSession)gpLinkedAccountModel.GpUserSession;
            
            try
            {
                EmisRequestParameters emisRequestParameters = gpLinkedAccountModel.BuildEmisRequestParameters(_logger);
                
                _logger.LogEnter();
                _logger.LogDebug("Beginning Fetch Courses for user");
                
                var coursesResponse = await _emisClient.CoursesGet(emisRequestParameters);
                
                _logger.LogDebug("Fetch Courses for user complete");

                if (coursesResponse.HasSuccessResponse)
                {
                    try
                    {
                        var totalCourses = coursesResponse.Body.Courses.Count();

                        _logger
                            .LogDebug("Filtering courses from successful emis response so we are left with only repeat courses which can be requested");

                        coursesResponse.Body.Courses = 
                            coursesResponse.Body.Courses
                            .Where(x => x.PrescriptionType == PrescriptionType.Repeat && x.CanBeRequested)
                            .OrderBy(x => x.Name);

                            _logger
                            .LogDebug($"Filtered courses from successful emis response so we are left with only repeat courses which can be requested");

                        var kvp = new Dictionary<string, string>
                        {
                            { "Total courses from response before filtering", totalCourses.ToString(CultureInfo.InvariantCulture) },
                            { "Total courses after filtering", coursesResponse.Body.Courses.Count().ToString(CultureInfo.InvariantCulture) }
                        };

                        await _auditor.Audit(AuditType, "Total courses before filtering: {0}, Total courses after filtering: {1}", 
                            totalCourses.ToString(CultureInfo.InvariantCulture), 
                            coursesResponse.Body.Courses.Count().ToString(CultureInfo.InvariantCulture));

                        _logger
                            .LogInformationKeyValuePairs("Filtering counts", kvp);
                        
                        if (_settings.CoursesMaxCoursesLimit.HasValue)
                        {
                            coursesResponse.Body.Courses = 
                                coursesResponse.Body.Courses
                                    .Take(_settings.CoursesMaxCoursesLimit.Value);
                        }


                        _logger.LogDebug($"Mapping response from {nameof(CoursesGetResponse)} to {nameof(CourseListResponse)}");
                        
                        var courseListResponse = _emisPrescriptionMapper.Map(coursesResponse.Body);
                        courseListResponse.SpecialRequestNecessity = emisUserSession.PrescriptionSpecialRequestNecessity;

                        return new GetCoursesResult.Success(courseListResponse);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Something went wrong while building the response");
                        return new GetCoursesResult.InternalServerError();
                    }
                }
                
                _logger.LogEmisUnknownError(coursesResponse);
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
            EmisClient.EmisApiResponse response)
        {  
            if (response.HasForbiddenResponse())
            {
                _logger.LogWarning("The emis prescriptions service is not enabled");
                _logger.LogEmisWarningResponse(response);
                return new GetCoursesResult.Forbidden();
            }
            
            _logger.LogError("Emis system is currently unavailable");
            _logger.LogEmisErrorResponse(response);
            return new GetCoursesResult.BadGateway();       
        }
    }
}
