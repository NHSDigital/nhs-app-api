using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Courses;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppCourseService : ICourseService
    {
        private readonly ILogger _logger;
        private readonly ConfigurationSettings _settings;
        private readonly ITppClient _tppClient;
        private readonly ITppCourseMapper _tppCourseMapper;

        public TppCourseService(ILoggerFactory loggerFactory, IOptions<ConfigurationSettings> settings,
            ITppClient tppClient, ITppCourseMapper tppCourseMapper)
        {
            _logger = loggerFactory.CreateLogger<TppCourseService>();
            _settings = settings.Value;
            _tppClient = tppClient;
            _tppCourseMapper = tppCourseMapper;
        }

        public async Task<GetCoursesResult> GetCourses(UserSession userSession)
        {
            var tppUserSession = (TppUserSession) userSession;

            try
            {
                var response = await _tppClient.ListRepeatMedicationPost(tppUserSession);

                if (response.HasSuccessResponse)
                {
                    try
                    {
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
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving repeat prescriptions");
                return new GetCoursesResult.SupplierSystemUnavailable();
            }

            return new GetCoursesResult.SupplierSystemUnavailable();
        }

        private List<Medication> GetMaxRequestablePrescriptions(List<Medication> medications)
        {
            const string IsRequestable = "y";

            return medications
                .Where(x => x.Requestable?.ToLower() == IsRequestable)
                .Take(_settings.CoursesMaxCoursesLimit.Value).ToList();
        }
    }
}