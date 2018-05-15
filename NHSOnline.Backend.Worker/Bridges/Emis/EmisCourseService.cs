using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Mappers;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Prescriptions;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisCourseService : ICourseService
    {
        private readonly IEmisClient _emisClient;
        private readonly IEmisPrescriptionMapper _emisPrescriptionMapper;
        private readonly ILogger _logger;

        public EmisCourseService(ILoggerFactory loggerFactory, IEmisClient emisClient, IEmisPrescriptionMapper emisPrescriptionMapper)
        {
            _emisClient = emisClient;
            _emisPrescriptionMapper = emisPrescriptionMapper;
            _logger = loggerFactory.CreateLogger<EmisCourseService>();
        }

        public async Task<GetCoursesResult> Get(UserSession userSession)
        {
            var emisUserSession = (EmisUserSession) userSession;

            try
            {
                var coursesResponse = await _emisClient.CoursesGet(emisUserSession.UserPatientLinkToken, emisUserSession.SessionId, emisUserSession.EndUserSessionId);

                if (!coursesResponse.HasSuccessStatusCode)
                {
                    _logger.LogError("Unsuccessful request retrieving courses");
                    return new GetCoursesResult.Unsuccessful();
                }

                _logger.LogDebug($"Mapping response from {nameof(CoursesGetResponse)} to {nameof(CourseListResponse)}");
                var result = _emisPrescriptionMapper.Map(coursesResponse.Body);

                return new GetCoursesResult.SuccessfullyRetrieved(result);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving courses");
                return new GetCoursesResult.Unsuccessful();
            }
        }
    }
}
