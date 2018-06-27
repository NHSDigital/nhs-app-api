using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public class EmisAppointmentsServiceGetAppointments 
    {
        private readonly IEmisClient _emisClient;
        private readonly IAppointmentsResponseMapper _responseMapper;
        private readonly ILogger _logger;

        public EmisAppointmentsServiceGetAppointments(
            ILogger logger, 
            IEmisClient emisClient,
            IAppointmentsResponseMapper responseMapper
            )
        {
            _emisClient = emisClient;
            _responseMapper = responseMapper;
            _logger = logger;
        }

        public async Task<AppointmentsResult> GetAppointments(
            UserSession userSession, 
            bool includePastAppointments,
            DateTimeOffset? pastAppointmentsFromDate)
        {
            var emisUserSession = (EmisUserSession) userSession;

            var emisHeaders = new EmisHeaderParameters(emisUserSession);

            try
            {
                var response = await _emisClient.AppointmentsGet(emisHeaders,
                    emisUserSession.UserPatientLinkToken,
                    includePastAppointments, 
                    pastAppointmentsFromDate);
                return InterpretAppointmentsGetResponse(response);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Getting appointments failed.");
                return new AppointmentsResult.SupplierSystemUnavailable();
            }
        }

        private AppointmentsResult InterpretAppointmentsGetResponse(
            EmisClient.EmisApiObjectResponse<AppointmentsGetResponse> response)
        {
            if (response.HasSuccessStatusCode)
            {
                try
                {
                    return new AppointmentsResult.SuccessfullyRetrieved(_responseMapper.Map(response.Body));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Something went wrong during building the response.");
                    return new AppointmentsResult.InternalServerError();
                }
            }

            if (response.HasForbiddenResponse())
            {
                _logger.LogEmisResponseIsForbidden();
                return new AppointmentsResult.CannotViewAppointments();
            }

            _logger.LogEmisUnknownError(response);
            return new AppointmentsResult.SupplierSystemUnavailable();
        }
    }
}