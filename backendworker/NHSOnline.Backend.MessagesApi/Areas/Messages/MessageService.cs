using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    internal class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<MessagesController> _logger;
        private readonly IMapper<List<UserMessage>, MessagesResponse> _userMessagesToResponseMapper;
        private readonly IMapper<List<SummaryMessage>, MessagesResponse> _summaryMessagesToResponseMapper;
        private readonly IMessagesValidationService _validator;

        public MessageService
        (
            IMessageRepository messageRepository,
            ILogger<MessagesController> logger,
            IMapper<List<UserMessage>, MessagesResponse> userMessagesToResponseMapper,
            IMapper<List<SummaryMessage>, MessagesResponse> summaryMessagesToResponseMapper,
            IMessagesValidationService validator)
        {
            _messageRepository = messageRepository;
            _logger = logger;
            _userMessagesToResponseMapper = userMessagesToResponseMapper;
            _summaryMessagesToResponseMapper = summaryMessagesToResponseMapper;
            _validator = validator;
        }

        public async Task<MessageResult> Send(AddMessageRequest addMessageRequest, string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                if (!_validator.IsMessageRequestValid(addMessageRequest, nhsLoginId))
                {
                    return new MessageResult.BadRequest();
                }

                var userMessage = new UserMessage
                {
                    NhsLoginId = nhsLoginId,
                    Sender = addMessageRequest.Sender,
                    Version = addMessageRequest.Version,
                    Body = addMessageRequest.Body,
                    SentTime = DateTime.UtcNow
                };

                var result = await _messageRepository.Create(userMessage);

                return result.Accept(new RepositoryCreateResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError($"Message Posting has failed with exception: {e}");
                return new MessageResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<MessagesResult> GetMessages(AccessToken accessToken, string sender)
        {
            _logger.LogEnter();

            try
            {
                var result = await _messageRepository.FindMessagesFromSender(accessToken.Subject, sender);

                return result.Accept(new RepositoryFindMessageResultVisitor(_userMessagesToResponseMapper));
            }
            catch (Exception e)
            {
                _logger.LogError($"Sender Messages Get has failed with exception: {e}");
                return new MessagesResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<MessagesResult> GetSummaryMessages(AccessToken accessToken)
        {
            _logger.LogEnter();

            try
            {
                var result = await _messageRepository.FindAllForUser(accessToken.Subject);

                return result.Accept(
                    new RepositoryFindMessagesResultToSummaryMessageVisitor(_summaryMessagesToResponseMapper));
            }
            catch (Exception e)
            {
                _logger.LogError($"Summary Messages Get has failed with exception: {e}");
                return new MessagesResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<MessagePatchResult> UpdateMessage(JsonPatchDocument<Message> messagePatchDocument,
            AccessToken accessToken, string messageId)
        {
            _logger.LogEnter();

            try
            {
                if (!_validator.IsPatchRequestValid(messagePatchDocument, messageId))
                {
                    return new MessagePatchResult.BadRequest();
                }

                var mapperStep = new UpdateMessageMapperStep(_logger).Map(messagePatchDocument);
                if (mapperStep.ProcessFinishedEarly(out var messageUpdateResult))
                {
                    return messageUpdateResult;
                }

                var updateResult =
                    await _messageRepository.UpdateOne(accessToken.Subject, messageId, mapperStep.Result);
                return updateResult.Accept(new RepositoryUpdateMessageResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError($"Message Get has failed with exception: {e}");
                return new MessagePatchResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}