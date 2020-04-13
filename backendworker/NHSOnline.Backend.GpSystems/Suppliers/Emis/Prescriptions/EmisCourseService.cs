using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions
{
    public class EmisCourseService : ICourseService
    {
        private readonly ILogger<EmisCourseService> _logger;
        private readonly EmisConfigurationSettings _settings;
        private readonly IEmisClient _emisClient;
        private readonly IEmisPrescriptionMapper _emisPrescriptionMapper;

        public EmisCourseService(
            ILogger<EmisCourseService> logger, 
            EmisConfigurationSettings settings, 
            IEmisClient emisClient, 
            IEmisPrescriptionMapper emisPrescriptionMapper)
        {
            _logger = logger;
            _settings = settings;
            _emisClient = emisClient;
            _emisPrescriptionMapper = emisPrescriptionMapper;

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
                        var courses = coursesResponse.Body.Courses;
                        var totalCourses = coursesResponse.Body.Courses.Count();

                        if (totalCourses != 0)
                        {
                            var mostRecentIssueCount = courses.Count(c => c.MostRecentIssueDate.HasValue);
                            var nextIssueDateCount = courses.Count(c => c.NextIssueDate.HasValue);
                            var reviewDateCount = courses.Count(c => c.ReviewDate.HasValue);
                        
                            var kvp = new Dictionary<string, string>
                            {
                                { "MostRecentIssueDate populated ", $" {mostRecentIssueCount} / {totalCourses}" },
                                { "NextIssueDate populated ",  $" {nextIssueDateCount} / {totalCourses}" },
                                { "ReviewDate populated ", $" {reviewDateCount} / {totalCourses}" }
                            };

                            _logger.LogInformationKeyValuePairs("Prescription date data logging", kvp); 
                        }

                        _logger
                            .LogDebug("Filtering courses from successful emis response so we are left with only repeat courses which can be requested");

                        coursesResponse.Body.Courses = 
                            coursesResponse.Body.Courses
                            .Where(x => x.PrescriptionType == PrescriptionType.Repeat && x.CanBeRequested)
                            .OrderBy(x => x.Name);

                        var numberOfRepeatCourses = coursesResponse.Body.Courses.Count();

                        if (_settings.CoursesMaxCoursesLimit.HasValue) 
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
                        
                        var courseListResponse = _emisPrescriptionMapper.Map(coursesResponse.Body);
                        courseListResponse.SpecialRequestNecessity = emisUserSession.PrescriptionSpecialRequestNecessity;

                        return new GetCoursesResult.Success(courseListResponse, coursesCount);
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
