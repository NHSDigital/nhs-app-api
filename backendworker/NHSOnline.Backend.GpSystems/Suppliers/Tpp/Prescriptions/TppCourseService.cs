using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions
{
    internal class TppCourseService : ICourseService
    {
        private readonly ITppClientRequest<TppRequestParameters, ListRepeatMedicationReply> _listRepeatMedication;
        private readonly ILogger<TppCourseService> _logger;
        private readonly TppConfigurationSettings _settings;
        private readonly ITppCourseMapper _tppCourseMapper;

        public TppCourseService(
            ITppClientRequest<TppRequestParameters, ListRepeatMedicationReply> listRepeatMedication,
            ILogger<TppCourseService> logger, 
            TppConfigurationSettings settings,
            ITppCourseMapper tppCourseMapper)
        {
            _listRepeatMedication = listRepeatMedication;
            _logger = logger;
            _settings = settings;
            _tppCourseMapper = tppCourseMapper;

            _settings.Validate();
        }

        public async Task<GetCoursesResult> GetCourses(GpLinkedAccountModel gpLinkedAccountModel)
        {
            TppRequestParameters tppRequestParameters = gpLinkedAccountModel.BuildTppRequestParameters(_logger);


            try
            {
                _logger.LogEnter();
                _logger.LogDebug("Beginning Fetch Courses for user");
                var response = await _listRepeatMedication.Post(tppRequestParameters);
                _logger.LogDebug("Fetch Courses for user complete");
                
                if (response.HasSuccessResponse)
                {
                    try
                    {
                        var totalCourses = response.Body.Medications.Count;

                        _logger
                            .LogDebug("Filtering courses from successful tpp response so we are left with only repeat courses which can be requested");
                        
                        var medications = response.Body.Medications.ToList();
                        
                        var requestableRepeatablePrescriptions = medications
                            .Where(x => TppApiConstants.MedicationRequestable.Yes.Equals(x.Requestable,
                                            StringComparison.OrdinalIgnoreCase) &&
                                        TppApiConstants.MedicationType.Repeat.Equals(x.Type, StringComparison.OrdinalIgnoreCase));
                        
                        var repeatCoursesAfterLimit = _settings.CoursesMaxCoursesLimit.HasValue ?
                            requestableRepeatablePrescriptions.Take(_settings.CoursesMaxCoursesLimit.Value) : requestableRepeatablePrescriptions;

                        var numberOfRequestableRepeatCourses = requestableRepeatablePrescriptions.Count();
                        
                        var medicationListFiltered = repeatCoursesAfterLimit.OrderBy(x => x.Drug).ToList();
                        var numberOfCoursesAfterFiltering = medicationListFiltered.Count;
                        
                        var numberOfCoursesDiscarded = numberOfRequestableRepeatCourses - numberOfCoursesAfterFiltering;
                        
                        var coursesCount = new FilteringCounts
                        {
                            ReceivedCount = totalCourses,
                            ReceivedRepeatsCount = numberOfRequestableRepeatCourses,
                            ExcessRepeatsCount = numberOfCoursesDiscarded,
                            ReturnedCount = numberOfCoursesAfterFiltering
                        };

                        _logger.LogDebug(
                            $"Mapping response from {nameof(ListRepeatMedicationReply)} to {nameof(CourseListResponse)}");
                        
                        var mapppedPrescriptionList = _tppCourseMapper.Map(medicationListFiltered);
                        return new GetCoursesResult.Success(mapppedPrescriptionList, coursesCount);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Something went wrong while building the response");
                        return new GetCoursesResult.InternalServerError();
                    }
                }
                                                      
                _logger.LogTppUnknownError(response);
                return GetCorrectErrorResult(response);
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
            TppApiResponse response)
        {
            if (response.HasForbiddenResponse)
            {
                _logger.LogError("The tpp prescriptions service is not enabled");

                return new GetCoursesResult.Forbidden();
            }

            _logger.LogError("Tpp system is currently unavailable");

            return new GetCoursesResult.BadGateway();
        }
    }
}
