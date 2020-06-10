using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public class EmisPatientMessagesService : IPatientMessagesService
    {
        private const NumberStyles POSTIVE_INTEGER = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

        private readonly ILogger<EmisPatientMessagesService> _logger;
        private readonly IEmisClient _emisClient;
        private readonly IEmisPatientMessagesMapper _messageListMapper;
        private readonly IEmisPatientMessageMapper _messageMapper;
        private readonly IEmisPatientMessageSendMapper _messageSendWrapper;
        private readonly IEmisPatientMessageRecipientsMapper _messageRecipientsMapper;

        public EmisPatientMessagesService(
            ILogger<EmisPatientMessagesService> logger,
            IEmisClient emisClient,
            IEmisPatientMessagesMapper messageListMapper,
            IEmisPatientMessageMapper messageMapper,
            IEmisPatientMessageSendMapper messageSendMapper,
            IEmisPatientMessageRecipientsMapper messageRecipientsMapper)
        {
            _logger = logger;
            _emisClient = emisClient;
            _messageListMapper = messageListMapper;
            _messageMapper = messageMapper;
            _messageSendWrapper = messageSendMapper;
            _messageRecipientsMapper = messageRecipientsMapper;
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
                return new GetPatientMessageResult.BadGateway();
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

        public async Task<PutPatientMessageReadStatusResult> UpdateMessageMessageReadStatus(GpUserSession gpUserSession, UpdateMessageReadStatusRequestBody updateRequest)
        {
            if (updateRequest is null)
            {
                throw new ArgumentNullException(nameof(updateRequest));
            }

            _logger.LogEnter();

            var emisRequestParameters = new EmisRequestParameters((EmisUserSession) gpUserSession);
            var putRequestBody = new UpdateMessageReadStatusRequest
            {
                MessageId = ParseMessageId(updateRequest),
                MessageReadState = updateRequest.MessageReadState,
                UserPatientLinkToken = emisRequestParameters.UserPatientLinkToken
            };

            try
            {
                var response = await _emisClient.PatientMessageUpdatePut(emisRequestParameters, putRequestBody);

                return InterpretPatientMessagePutResponse(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Request to update patient message status with id {updateRequest.MessageId} has failed");
                return new PutPatientMessageReadStatusResult.BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unknown error occured when updating patient message status with id {updateRequest.MessageId} has failed");
                return new PutPatientMessageReadStatusResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<GetPatientMessageRecipientsResult> GetMessageRecipients(GpUserSession gpUserSession)
        {
            _logger.LogEnter();

            try
            {
                var response = await _emisClient.PatientMessageRecipientsGet(new EmisRequestParameters((EmisUserSession) gpUserSession));

                return InterpretPatientMessageRecipientsGetResponse(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Request to get patient message recipients was unsuccessful");
                return new GetPatientMessageRecipientsResult.BadGateway();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown error occured when getting patient message recipients");
                return new GetPatientMessageRecipientsResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<PostPatientMessageResult> SendMessage(GpUserSession gpUserSession,
            CreatePatientMessage message)
        {
            try
            {
                var response = await _emisClient.PatientMessagePost(
                    new EmisRequestParameters((EmisUserSession) gpUserSession), message);

                return InterpretSendMessageResponse(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Request to send patient message was unsuccessful");
                return new PostPatientMessageResult.BadGateway();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unknown error occured when sending the patient message");
                return new PostPatientMessageResult.InternalServerError();
            }
        }

        public async Task<DeletePatientMessageResult> DeleteMessage(GpUserSession gpUserSession, string messageId)
        {
            try
            {
                var response = await _emisClient.PatientPracticeMessageDelete(
                    new EmisRequestParameters((EmisUserSession) gpUserSession), messageId);

                _logger.LogInformation($"Patient practice message with id {messageId} has been deleted");
                return InterpretDeleteMessageResponse(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Request to delete a patient message with id {messageId} was unsuccessful");
                return new DeletePatientMessageResult.BadGateway();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unknown error occured when deleting the patient message with id {messageId}");
                return new DeletePatientMessageResult.InternalServerError();
            }
        }

        private GetPatientMessagesResult InterpretPatientMessagesGetResponse(
        EmisApiObjectResponse<MessagesGetResponse> response)
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
            EmisApiObjectResponse<MessageGetResponse> response)
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

                if (mapped.MessageDetails.Replies != null)
                {
                    _logger.LogInformation($"Number of replies for message with id {mapped.MessageDetails.MessageId} is {mapped.MessageDetails.Replies.Count}");
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

        private PutPatientMessageReadStatusResult InterpretPatientMessagePutResponse(
            EmisApiObjectResponse<MessageUpdateResponse> response)
        {
            if (response.HasSuccessResponse)
            {
                return new PutPatientMessageReadStatusResult.Success();
            }

            if (response.HasForbiddenResponse())
            {
                _logger.LogEmisResponseIsForbidden();
                _logger.LogEmisErrorResponse(response);
                return new PutPatientMessageReadStatusResult.Forbidden();
            }

            if (response.HasBadRequestResponse)
            {
                _logger.LogEmisErrorResponse(response);
                return new PutPatientMessageReadStatusResult.BadRequest();
            }

            _logger.LogEmisUnknownError(response);
            _logger.LogEmisErrorResponse(response);

            return new PutPatientMessageReadStatusResult.BadGateway();
        }

        private GetPatientMessageRecipientsResult InterpretPatientMessageRecipientsGetResponse(
            EmisApiObjectResponse<MessageRecipientsResponse> response)
        {
            if (response.HasSuccessResponse)
            {
                var messageRecipientsResponse = response.Body;

                _logger.LogInformation($"Number of recipients: {messageRecipientsResponse?.MessageRecipients?.Count}");
                return new GetPatientMessageRecipientsResult.Success(_messageRecipientsMapper.Map(messageRecipientsResponse));
            }

            if (response.HasForbiddenResponse())
            {
                _logger.LogEmisResponseIsForbidden();
                _logger.LogEmisErrorResponse(response);
                return new GetPatientMessageRecipientsResult.Forbidden();
            }

            if (response.HasBadRequestResponse)
            {
                _logger.LogEmisErrorResponse(response);
                return new GetPatientMessageRecipientsResult.BadRequest();
            }

            _logger.LogEmisUnknownError(response);
            _logger.LogEmisErrorResponse(response);
            return new GetPatientMessageRecipientsResult.BadGateway();
        }

        private PostPatientMessageResult InterpretSendMessageResponse(
            EmisApiObjectResponse<MessagePostResponse> response)
        {
            if (response.HasSuccessResponse)
            {
                var mapped = _messageSendWrapper.Map(response.Body);
                if (mapped.HasValue)
                {
                    return new PostPatientMessageResult.Success();
                }

                return new PostPatientMessageResult.BadRequest();
            }

            if (response.HasForbiddenResponse())
            {
                _logger.LogEmisResponseIsForbidden();
                _logger.LogEmisErrorResponse(response);
                return new PostPatientMessageResult.Forbidden();
            }

            if (response.HasBadRequestResponse)
            {
                _logger.LogEmisErrorResponse(response);
                return new PostPatientMessageResult.BadRequest();
            }

            _logger.LogEmisUnknownError(response);
            _logger.LogEmisErrorResponse(response);
            return new PostPatientMessageResult.BadGateway();
        }

        private DeletePatientMessageResult InterpretDeleteMessageResponse(
            EmisApiObjectResponse<MessageDeleteResponse> response)
        {
            if (response.HasSuccessResponse)
            {
                if (response.Body?.IsDeleted != null && !response.Body.IsDeleted.Value)
                {
                    return new DeletePatientMessageResult.BadGateway();
                }

                return new DeletePatientMessageResult.Success();
            }

            if (response.HasForbiddenResponse())
            {
                _logger.LogEmisResponseIsForbidden();
                _logger.LogEmisErrorResponse(response);
                return new DeletePatientMessageResult.Forbidden();
            }

            if (response.HasBadRequestResponse)
            {
                _logger.LogEmisErrorResponse(response);
                return new DeletePatientMessageResult.BadRequest();
            }

            _logger.LogEmisUnknownError(response);
            _logger.LogEmisErrorResponse(response);
            return new DeletePatientMessageResult.BadGateway();
        }

        private int ParseMessageId(UpdateMessageReadStatusRequestBody updateRequest)
        {
            var messageIdWasParsed = int.TryParse(
                updateRequest.MessageId,
                POSTIVE_INTEGER,
                CultureInfo.InvariantCulture,
                out var messageId);

            if (!messageIdWasParsed)
            {
                throw new ArgumentException($"Field {nameof(updateRequest.MessageId)} in " +
                                            $"parameter {nameof(updateRequest)} must be a valid positive integer");
            }

            return messageId;
        }
    }
}
