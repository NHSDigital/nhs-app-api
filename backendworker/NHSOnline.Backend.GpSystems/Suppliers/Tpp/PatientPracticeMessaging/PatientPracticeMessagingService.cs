using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging
{
    internal sealed class PatientPracticeMessagingService : IPatientMessagesService
    {
        private readonly ILogger<TppPatientRecordService> _logger;
        private readonly ITppClientRequest<TppUserSession, MessageRecipientsReply> _messageRecipients;
        private readonly IGetPatientPracticeMessagingRecipientsTaskChecker _messageRecipientsTaskChecker;
        private readonly ITppClientRequest<TppUserSession, MessagesViewReply> _messagesViewPost;
        private readonly ITppPatientMessagesMapper _mapper;

        public PatientPracticeMessagingService(
            ILogger<TppPatientRecordService> logger,
            ITppClientRequest<TppUserSession, MessageRecipientsReply> messageRecipients,
            IGetPatientPracticeMessagingRecipientsTaskChecker messageRecipientsTaskChecker,
            ITppPatientMessagesMapper mapper,
            ITppClientRequest<TppUserSession, MessagesViewReply> messagesViewPost)
        {
            _logger = logger;
            _messageRecipients = messageRecipients;
            _messageRecipientsTaskChecker = messageRecipientsTaskChecker;
            _mapper = mapper;
            _messagesViewPost = messagesViewPost;
        }

        public async Task<GetPatientMessagesResult> GetMessages(GpUserSession gpUserSession)
        {
            _logger.LogEnter();

            var tppUserSession = (TppUserSession) gpUserSession;
            try
            {
                var response = await _messagesViewPost.Post(tppUserSession);
                if (response.HasSuccessResponse)
                {
                    _logger.LogInformation("Mapping TPP patient messages");
                    var mapped = _mapper.Map(response.Body);

                    if (mapped == null)
                    {
                        _logger.LogInformation("Mapping TPP patient messages returned null");
                        return new GetPatientMessagesResult.BadGateway();
                    }

                    _logger.LogInformation($"Number of TPP patient messages retrieved: {mapped.MessageSummaries.Count}");

                    return new GetPatientMessagesResult.Success(mapped);
                }

                if (response.HasErrorWithCode(TppApiErrorCodes.NoAccess))
                {
                    _logger.LogTppResponseAccessIsForbidden();
                    return new GetPatientMessagesResult.Forbidden();
                }

                _logger.LogTppErrorResponse(response);
                return new GetPatientMessagesResult.BadGateway();

            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Request to get patient messages was unsuccessful");
                return new GetPatientMessagesResult.BadGateway();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown error occured when getting patient messages");
                return new GetPatientMessagesResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public Task<GetPatientMessageResult> GetMessageDetails(string messageId, GpUserSession gpUserSession)
        {
            throw new System.NotImplementedException();
        }

        public Task<PutPatientMessageReadStatusResult> UpdateMessageMessageReadStatus(GpUserSession gpUserSession, UpdateMessageReadStatusRequestBody updateRequest)
        {
            throw new System.NotImplementedException();
        }

        public async Task<GetPatientMessageRecipientsResult> GetMessageRecipients(GpUserSession gpUserSession)
        {
            _logger.LogEnter();

            var tppUserSession = (TppUserSession)gpUserSession;

            try
            {
                var messageRecipients = await RetrieveMessageRecipients(tppUserSession);

                if (messageRecipients.HasErrored)
                {
                    _logger.LogExitWith($"{nameof(messageRecipients.HasErrored)}=true");
                    return new GetPatientMessageRecipientsResult.BadGateway();
                }

                _logger.LogInformation($"Number of recipients for user " +
                                       $"at ODSCode {tppUserSession.OdsCode} is " +
                                       $"{messageRecipients.MessageRecipients.Count}");

                return new GetPatientMessageRecipientsResult.Success(messageRecipients);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving patient practice messaging recipients");
                return new GetPatientMessageRecipientsResult.BadGateway();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "Patient practice messaging recipients retrieval returned a null body");
                return new GetPatientMessageRecipientsResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public Task<PostPatientMessageResult> SendMessage(GpUserSession gpUserSession, CreatePatientMessage message)
        {
            throw new System.NotImplementedException();
        }

        public Task<DeletePatientMessageResult> DeleteMessage(GpUserSession gpUserSession, string messageId)
        {
            throw new System.NotImplementedException();
        }

        private async Task<PatientPracticeMessageRecipients> RetrieveMessageRecipients(TppUserSession tppUserSession)
        {
            try
            {
                var messageRecipients = await _messageRecipients.Post(tppUserSession);
                _logger.LogDebug($"Mapping messaging recipients response to {new MessageRecipient()} classes");
                return _messageRecipientsTaskChecker.Check(messageRecipients);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Retrieving message recipients has failed. Returning HasErrored as true");
                return new PatientPracticeMessageRecipients
                {
                    HasErrored = true
                };
            }
        }
    }
}