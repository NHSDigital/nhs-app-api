using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions
{
    public class TppCourseService : ICourseService
    {
        private readonly ILogger<TppCourseService> _logger;
        private readonly ConfigurationSettings _settings;
        private readonly ITppClient _tppClient;
        private readonly ITppCourseMapper _tppCourseMapper;

        public TppCourseService(ILogger<TppCourseService> logger, IOptions<ConfigurationSettings> settings,
            ITppClient tppClient, ITppCourseMapper tppCourseMapper)
        {
            _logger = logger;
            _settings = settings.Value;
            _tppClient = tppClient;
            _tppCourseMapper = tppCourseMapper;
        }

        public async Task<GetCoursesResult> GetCourses(UserSession userSession)
        {
            var tppUserSession = (TppUserSession) userSession;

            try
            {
                _logger.LogEnter();
                _logger.LogDebug("Beginning Fetch Courses for user");
                var response = await _tppClient.ListRepeatMedicationPost(tppUserSession);
                _logger.LogDebug("Fetch Courses for user complete");
                
                if (response.HasSuccessResponse)
                {
                    try
                    {
                        _logger
                            .LogDebug("Filtering courses from successful emis response so we are left with only repeat courses which can be requested");
                        
                        var medicationListFiltered = GetMaxRequestablePrescriptions(response.Body.Medications.ToList());
                        
                        _logger.LogDebug(
                            $"Mapping response from {nameof(ListRepeatMedicationReply)} to {nameof(CourseListResponse)}");
                        
                        var mapppedPrescriptionList = _tppCourseMapper.Map(medicationListFiltered);
                        return new GetCoursesResult.SuccessfullyRetrieved(mapppedPrescriptionList);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(
                            $"Something went wrong during building the response. Exception message: {e.Message}");

                        return new GetCoursesResult.InternalServerError();
                    }
                }
                                                      
                _logger.LogTppUnknownError(response);
                return GetCorrectErrorResult(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving repeat prescriptions");
                return new GetCoursesResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private List<Medication> GetMaxRequestablePrescriptions(IEnumerable<Medication> medications)
        {
            var requestableRepeatablePrescriptions = medications
                .Where(x => TppApiConstants.MedicationRequestable.Yes.Equals(x.Requestable,
                                StringComparison.OrdinalIgnoreCase) &&
                            TppApiConstants.MedicationType.Repeat.Equals(x.Type, StringComparison.OrdinalIgnoreCase));

            if (_settings.CoursesMaxCoursesLimit != null)
            {
                requestableRepeatablePrescriptions = 
                    requestableRepeatablePrescriptions
                        .Take(_settings.CoursesMaxCoursesLimit.Value);
            }

            return requestableRepeatablePrescriptions.OrderBy(x => x.Drug).ToList();
        }

        private GetCoursesResult GetCorrectErrorResult(
            TppClient.TppApiResponse response)
        {
            if (response.HasForbiddenResponse)
            {
                _logger.LogError("The tpp prescriptions service is not enabled");

                return new GetCoursesResult.SupplierNotEnabled();
    }

            _logger.LogError("Tpp system is currently unavailable");

            return new GetCoursesResult.SupplierSystemUnavailable();
        }
    }
}
