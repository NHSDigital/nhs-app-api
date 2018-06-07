using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Mappers;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
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

        public async Task<GetCoursesResult> Get(UserSession userSession)
        {
            var emisUserSession = (EmisUserSession) userSession;

            try
            {
                var coursesResponse = await _emisClient.CoursesGet(emisUserSession.UserPatientLinkToken, emisUserSession.SessionId, emisUserSession.EndUserSessionId);

                if (!coursesResponse.HasSuccessStatusCode)
                {
                    _logger.LogError($"Unsuccessful request retrieving courses. Status code: {(int)coursesResponse.StatusCode}");
                    return new GetCoursesResult.Unsuccessful();
                }

                _logger.LogDebug("Filtering courses from emis so we are left with only repeat courses which can be requested");
                coursesResponse.Body.Courses = coursesResponse.Body.Courses
                    .Where(x => x.PrescriptionType == PrescriptionType.Repeat && x.CanBeRequested)
                    .OrderBy(x => x.Name)
                    .Take(_settings.CoursesMaxCoursesLimit.Value);

                _logger.LogDebug($"Mapping response from {nameof(CoursesGetResponse)} to {nameof(CourseListResponse)}");
                var result = _emisPrescriptionMapper.Map(coursesResponse.Body);

                return new GetCoursesResult.SuccessfullyRetrieved(result);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving courses");
                return new GetCoursesResult.Unsuccessful();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "Prescription retrieval return null body");
                return new GetCoursesResult.SupplierBadData();
            }
        }
    }
}
