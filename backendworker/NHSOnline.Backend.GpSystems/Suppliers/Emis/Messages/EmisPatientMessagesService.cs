using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public class EmisPatientMessagesService : IPatientMessagesService
    {
        private const NumberStyles PositiveInteger = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

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

                return new GetPatientMessagesEmisApiResponseHandler(_logger, _messageListMapper).Handle(response);
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

                return new GetPatientMessageEmisApiResponseHandler(_logger, _messageMapper).Handle(response);
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

        public async Task<PutPatientMessageReadStatusResult> UpdateMessageMessageReadStatus(
            GpUserSession gpUserSession,
            UpdateMessageReadStatusRequestBody updateRequest)
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

                return new PutPatientMessageReadStatusEmisApiResponseHandler(_logger).Handle(response);
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

                return new GetPatientMessageRecipientsEmisApiResponseHandler(_logger, _messageRecipientsMapper).Handle(response);
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

        public async Task<PostPatientMessageResult> SendMessage(
            GpUserSession gpUserSession,
            CreatePatientMessage message)
        {
            try
            {
                var response = await _emisClient.PatientMessagePost(
                    new EmisRequestParameters((EmisUserSession) gpUserSession),
                    message);

                return new PostPatientMessageEmisApiResponseHandler(_logger, _messageSendWrapper).Handle(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Request to send patient message was unsuccessful");
                return new PostPatientMessageResult.BadGateway();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown error occured when sending the patient message");
                return new PostPatientMessageResult.InternalServerError();
            }
        }

        public async Task<DeletePatientMessageResult> DeleteMessage(GpUserSession gpUserSession, string messageId)
        {
            try
            {
                var response = await _emisClient.PatientPracticeMessageDelete(
                    new EmisRequestParameters((EmisUserSession) gpUserSession),
                    messageId);

                _logger.LogInformation($"Patient practice message with id {messageId} has been deleted");
                return new DeletePatientMessageEmisApiResponseHandler(_logger).Handle(response);
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

        private static int ParseMessageId(UpdateMessageReadStatusRequestBody updateRequest)
        {
            var messageIdWasParsed = int.TryParse(
                updateRequest.MessageId,
                PositiveInteger,
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
