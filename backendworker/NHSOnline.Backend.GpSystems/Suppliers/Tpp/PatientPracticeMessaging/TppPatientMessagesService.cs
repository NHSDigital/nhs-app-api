using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging
{
    internal sealed class TppPatientMessagesService : IPatientMessagesService
    {
        private static readonly Type GET_MESSAGES_SUCCESS_TYPE = typeof(GetPatientMessagesResult.Success);
        private static readonly Type GET_MESSAGES_FORBIDDEN_TYPE = typeof(GetPatientMessagesResult.Forbidden);

        private readonly ILogger<TppPatientRecordService> _logger;
        private readonly ITppClientRequest<TppUserSession, MessageRecipientsReply> _listRecipientsRequest;
        private readonly IGetPatientPracticeMessagingRecipientsTaskChecker _messageRecipientsTaskChecker;
        private readonly ITppClientRequest<TppUserSession, MessagesViewReply> _viewMessageRequest;
        private readonly ITppClientRequest<(TppRequestParameters, List<string>), MessagesMarkAsReadReply>
            _markMessageAsReadRequest;
        private readonly ITppPatientMessagesMapper _messagesViewMapper;
        private readonly ITppPatientMessagesUnreadIdsMapper _unreadMessagesWrapper;
        private ITppClientRequest<(TppUserSession tppUserSession, string recipientIdentifier, string messageText),
            MessageCreateReply> _messagesCreateMessagePost;

        public TppPatientMessagesService(
            ILogger<TppPatientRecordService> logger,
            ITppClientRequest<TppUserSession, MessageRecipientsReply> listRecipientsRequest,
            IGetPatientPracticeMessagingRecipientsTaskChecker messageRecipientsTaskChecker,
            ITppPatientMessagesMapper messagesViewMapper,
            ITppClientRequest<TppUserSession, MessagesViewReply> viewMessageRequest,
            ITppClientRequest<(TppRequestParameters, List<string>), MessagesMarkAsReadReply> markMessageAsReadRequest,
            ITppPatientMessagesUnreadIdsMapper unreadMessagesWrapper,
            ITppClientRequest<(TppUserSession tppUserSession, string recipientIdentifier,
                string messageText), MessageCreateReply> messagesCreateMessagePost)
        {
            _logger = logger;
            _listRecipientsRequest = listRecipientsRequest;
            _messageRecipientsTaskChecker = messageRecipientsTaskChecker;
            _messagesViewMapper = messagesViewMapper;
            _viewMessageRequest = viewMessageRequest;
            _markMessageAsReadRequest = markMessageAsReadRequest;
            _unreadMessagesWrapper = unreadMessagesWrapper;
            _messagesCreateMessagePost = messagesCreateMessagePost;
        }

        public async Task<GetPatientMessagesResult> GetMessages(GpUserSession gpUserSession)
        {
            _logger.LogEnter();

            try
            {
                var tppUserSession = CastUserSession(gpUserSession);

                var (result, response) = await GetMessagesForSession(tppUserSession);

                if (!(result is null))
                {
                    return result;
                }

                _logger.LogInformation("Mapping TPP patient messages");

                var mappedResponse = _messagesViewMapper.Map(response);

                if (mappedResponse is null)
                {
                    _logger.LogInformation("Mapping TPP patient messages returned null");
                    return new GetPatientMessagesResult.BadGateway();
                }

                _logger.LogInformation(
                    $"Number of TPP patient messages retrieved: {mappedResponse.MessageSummaries.Count}");

                return new GetPatientMessagesResult.Success(mappedResponse);
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
            throw new NotImplementedException("TPP does not require a separate call to get message details");
        }

        public async Task<PutPatientMessageReadStatusResult> UpdateMessageMessageReadStatus(
            GpUserSession gpUserSession,
            UpdateMessageReadStatusRequestBody updateRequest)
        {
            _logger.LogEnter();

            if (string.IsNullOrWhiteSpace(updateRequest?.MessageId))
            {
                throw new ArgumentException($"Field {nameof(updateRequest.MessageId)} in parameter " +
                                            $"{nameof(updateRequest)} must not be empty.");
            }

            try
            {
                var tppUserSession = CastUserSession(gpUserSession);

                return await MarkConversationMessagesAsRead(tppUserSession, updateRequest.MessageId);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request marking patient practice message as read");

                return new PutPatientMessageReadStatusResult.BadGateway();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown error occured when marking patient practice message as read");

                return new PutPatientMessageReadStatusResult.BadGateway();
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
                var tppUserSession = CastUserSession(gpUserSession);

                return await GetMessageRecipientsForSession(tppUserSession);
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

        public async Task<PostPatientMessageResult> SendMessage(GpUserSession gpUserSession, CreatePatientMessage message)
        {
            _logger.LogEnter();

            try
            {
                var tppUserSession = (TppUserSession) gpUserSession;
                var parameters = (tppUserSession, message.RecipientIdentifier, message.MessageBody);
                var response = await _messagesCreateMessagePost.Post(parameters);

                if (response.Body is null)
                {
                    _logger.LogTppErrorResponse(response);
                    return new PostPatientMessageResult.BadGateway();
                }

                return new PostPatientMessageResult.Success();

            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request post message patient practice messaging");
                return new PostPatientMessageResult.BadGateway();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "Patient practice messaging message create reply returned a null body");
                return new PostPatientMessageResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public Task<DeletePatientMessageResult> DeleteMessage(GpUserSession gpUserSession, string messageId)
        {
            throw new NotImplementedException();
        }

        private async Task<PutPatientMessageReadStatusResult> MarkConversationMessagesAsRead(
            TppUserSession tppUserSession,
            string conversationId)
        {
            var (userMessagesResult, userMessagesResponse) = await GetMessagesForSession(tppUserSession);
            var responseType = userMessagesResult?.GetType() ?? GET_MESSAGES_SUCCESS_TYPE;

            if (responseType != GET_MESSAGES_SUCCESS_TYPE)
            {
                return HandleGetMessagesForUnreadError(responseType);
            }

            var messageIdsToMarkAsRead = _unreadMessagesWrapper.Map(
                userMessagesResponse.Messages, conversationId);
            var requestParams = new TppRequestParameters(tppUserSession);

            _logger.LogInformation("Number of unread messages in conversation for user at ODSCode " +
                                   $"{tppUserSession.OdsCode} is {messageIdsToMarkAsRead.Count}");

            var response = await _markMessageAsReadRequest.Post((requestParams, messageIdsToMarkAsRead));

            if (!response.HasSuccessResponse)
            {
                return HandleMarkAsReadError(response);
            }

            return new PutPatientMessageReadStatusResult.Success();
        }

        private async Task<(GetPatientMessagesResult, MessagesViewReply)> GetMessagesForSession(
            TppUserSession tppUserSession)
        {
            var response = await _viewMessageRequest.Post(tppUserSession);

            if (response.HasForbiddenResponse)
            {
                _logger.LogTppResponseAccessIsForbidden();

                return (new GetPatientMessagesResult.Forbidden(), null);
            }

            if (!response.HasSuccessResponse)
            {
                _logger.LogTppErrorResponse(response);

                return (new GetPatientMessagesResult.BadGateway(), null);
            }

            return (null, response.Body);
        }

        private PutPatientMessageReadStatusResult HandleGetMessagesForUnreadError(Type responseType)
        {
            _logger.LogError("Failed to get unread messages for patient practice conversation " +
                             $"(to mark messages as read). Get messages response: {responseType}");

            if (responseType == GET_MESSAGES_FORBIDDEN_TYPE)
            {
                return new PutPatientMessageReadStatusResult.Forbidden();
            }

            return new PutPatientMessageReadStatusResult.BadGateway();
        }

        private PutPatientMessageReadStatusResult HandleMarkAsReadError(TppApiObjectResponse<MessagesMarkAsReadReply> response)
        {
            _logger.LogError("Marking messages as read failed. Status code: " +
                             $"{(int)response.StatusCode}");

            if (response.HasForbiddenResponse)
            {
                return new PutPatientMessageReadStatusResult.Forbidden();
            }

            return new PutPatientMessageReadStatusResult.BadGateway();
        }

        private async Task<GetPatientMessageRecipientsResult> GetMessageRecipientsForSession(TppUserSession tppUserSession)
        {
            var messageRecipients = await RetrieveMessageRecipients(tppUserSession);

            if (messageRecipients.HasErrored)
            {
                _logger.LogExitWith($"{nameof(messageRecipients.HasErrored)}=true");

                return new GetPatientMessageRecipientsResult.BadGateway();
            }

            _logger.LogInformation("Number of recipients for user " +
                                   $"at ODSCode {tppUserSession.OdsCode} is " +
                                   $"{messageRecipients.MessageRecipients.Count}");

            return new GetPatientMessageRecipientsResult.Success(messageRecipients);
        }

        private async Task<PatientPracticeMessageRecipients> RetrieveMessageRecipients(TppUserSession tppUserSession)
        {
            try
            {
                var messageRecipients = await _listRecipientsRequest.Post(tppUserSession);

                _logger.LogDebug($"Mapping messaging recipients response to {nameof(MessageRecipient)} instances");

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

        private TppUserSession CastUserSession(GpUserSession gpUserSession)
        {
            var userSession = gpUserSession as TppUserSession;

            if (userSession is null)
            {
                throw new InvalidOperationException($"{nameof(TppPatientMessagesService)} " +
                                                    $"methods must only be called with a non-null user session of " +
                                                    $"type {nameof(TppUserSession)} ");
            }

            return userSession;
        }
    }
}