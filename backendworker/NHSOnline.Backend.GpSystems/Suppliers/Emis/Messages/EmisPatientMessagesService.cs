using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public class EmisPatientMessagesService : IPatientMessagesService
    {
        private readonly ILogger<EmisPatientMessagesService> _logger;
        private readonly IEmisClient _emisClient;
        private readonly IEmisPatientMessagesMapper _messageListMapper;
        private readonly IEmisPatientMessageMapper _messageMapper;

        public EmisPatientMessagesService(
            ILogger<EmisPatientMessagesService> logger,
            IEmisClient emisClient,
            IEmisPatientMessagesMapper messageListMapper,
            IEmisPatientMessageMapper messageMapper)
        {
            _logger = logger;
            _emisClient = emisClient;
            _messageListMapper = messageListMapper;
            _messageMapper = messageMapper;
        }

        public async Task<GetPatientMessagesResult> GetMessages(GpUserSession gpUserSession)
        {
            _logger.LogEnter();

            try
            {
                var response = await _emisClient.PatientMessagesGet(new EmisRequestParameters((EmisUserSession) gpUserSession));

                return InterpretPatientMessagesGetResponse(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Request to get patient message summaries was unsuccessful");
                return new GetPatientMessagesResult.BadGateway();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown error occured when getting patient message summaries");
                return new GetPatientMessagesResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<GetPatientMessageResult> GetMessageDetails(string messageId, GpUserSession gpUserSession)
        {
            _logger.LogEnter();

            try
            {
                var response = await _emisClient.PatientMessageDetailsGet(messageId, new EmisRequestParameters((EmisUserSession) gpUserSession));

                return InterpretPatientMessageGetResponse(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Request to get patient message was unsuccessful with id id {messageId}");
                return new GetPatientMessageResult.BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unknown error occured when getting patient message with id {messageId}");
                return new GetPatientMessageResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private GetPatientMessagesResult InterpretPatientMessagesGetResponse(
            EmisClient.EmisApiObjectResponse<MessagesGetResponse> response)
        {
            if (response.HasSuccessResponse)
            {
                _logger.LogInformation("Mapping EMIS patient message summaries");
                var mapped = _messageListMapper.Map(response.Body);

                if (mapped == null)
                {
                    _logger.LogInformation("Mapping EMIS patient message summaries returned null");
                    return new GetPatientMessagesResult.BadGateway();
                }

                _logger.LogInformation($"Number of EMIS patient message summaries retrieved: {mapped.MessageSummaries.Count}");

                return new GetPatientMessagesResult.Success(mapped);
            }

            if (response.HasForbiddenResponse())
            {
                _logger.LogEmisResponseIsForbidden();
                _logger.LogEmisErrorResponse(response);
                return new GetPatientMessagesResult.Forbidden();
            }

            if (response.HasBadRequestResponse)
            {
                _logger.LogEmisErrorResponse(response);
                return new GetPatientMessagesResult.BadRequest();
            }

            _logger.LogEmisUnknownError(response);
            _logger.LogEmisErrorResponse(response);
            return new GetPatientMessagesResult.BadGateway();
        }

        private GetPatientMessageResult InterpretPatientMessageGetResponse(
            EmisClient.EmisApiObjectResponse<MessageGetResponse> response)
        {
            if (response.HasSuccessResponse)
            {
                _logger.LogInformation("Mapping EMIS patient message details");
                var mapped = _messageMapper.Map(response.Body);

                if (mapped == null)
                {
                    _logger.LogInformation("Mapping EMIS patient message details returned null");
                    return new GetPatientMessageResult.BadGateway();
                }

                if (mapped.MessageDetails.MessageReplies != null)
                {
                    _logger.LogInformation($"Number of replies for message with id {mapped.MessageDetails.MessageId} is {mapped.MessageDetails.MessageReplies.Count}");
                }
                return new GetPatientMessageResult.Success(mapped);
            }

            if (response.HasForbiddenResponse())
            {
                _logger.LogEmisResponseIsForbidden();
                _logger.LogEmisErrorResponse(response);
                return new GetPatientMessageResult.Forbidden();
            }

            if (response.HasBadRequestResponse)
            {
                _logger.LogEmisErrorResponse(response);
                return new GetPatientMessageResult.BadRequest();
            }

            _logger.LogEmisUnknownError(response);
            _logger.LogEmisErrorResponse(response);
            return new GetPatientMessageResult.BadGateway();
        }
    }
}
