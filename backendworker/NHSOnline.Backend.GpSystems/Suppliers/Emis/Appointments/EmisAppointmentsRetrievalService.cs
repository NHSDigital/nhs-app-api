using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments
{
    public class EmisAppointmentsRetrievalService
    {
        private readonly IEmisClient _emisClient;
        private readonly IAppointmentsResponseMapper _responseMapper;
        private readonly ILogger<EmisAppointmentsRetrievalService> _logger;

        public EmisAppointmentsRetrievalService(
            ILogger<EmisAppointmentsRetrievalService> logger,
            IEmisClient emisClient,
            IAppointmentsResponseMapper responseMapper
            )
        {
            _emisClient = emisClient;
            _responseMapper = responseMapper;
            _logger = logger;
        }

        public async Task<AppointmentsResult> GetAppointments(
            GpUserSession userSession)
        {
            try
            {
                _logger.LogEnter();
            
                var emisUserSession = (EmisUserSession) userSession;
                var emisRequestParameters = new EmisRequestParameters(emisUserSession);
                
                var response = await _emisClient.AppointmentsGet(
                    emisRequestParameters,
                    emisUserSession.UserPatientLinkToken);
                return InterpretAppointmentsGetResponse(response);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Getting appointments failed.");
                return new AppointmentsResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private AppointmentsResult InterpretAppointmentsGetResponse(
            EmisClient.EmisApiObjectResponse<AppointmentsGetResponse> response)
        {
            if (response.HasSuccessResponse)
            {
                try
                {
                    return new AppointmentsResult.Success(_responseMapper.Map(response.Body));
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
                _logger.LogEmisErrorResponse(response);
                return new AppointmentsResult.Forbidden();
            }

            _logger.LogEmisUnknownError(response);
            _logger.LogEmisErrorResponse(response);
            return new AppointmentsResult.BadGateway();
        }
    }
}
