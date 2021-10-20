using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<MessageService> _logger;
        private readonly IMapper<List<UserMessage>, MessagesResponse> _userMessagesToResponseMapper;
        private readonly IMapper<UserMessage, MessagesResponse> _userMessageToResponseMapper;
        private readonly IMapper<List<SummaryMessage>, MessagesResponse> _summaryMessagesToResponseMapper;
        private readonly IMapper<AddMessageRequest, string, UserMessage> _addMessageToUserMessageMapper;
        private readonly IMessagesValidationService _validator;

        public MessageService
        (
            IMessageRepository messageRepository,
            ILogger<MessageService> logger,
            IMapper<List<UserMessage>, MessagesResponse> userMessagesToResponseMapper,
            IMapper<UserMessage, MessagesResponse> userMessageToResponseMapper,
            IMapper<List<SummaryMessage>, MessagesResponse> summaryMessagesToResponseMapper,
            IMapper<AddMessageRequest, string, UserMessage> addMessageToUserMessageMapper,
            IMessagesValidationService validator)
        {
            _messageRepository = messageRepository;
            _logger = logger;
            _userMessagesToResponseMapper = userMessagesToResponseMapper;
            _userMessageToResponseMapper = userMessageToResponseMapper;
            _summaryMessagesToResponseMapper = summaryMessagesToResponseMapper;
            _addMessageToUserMessageMapper = addMessageToUserMessageMapper;
            _validator = validator;
        }

        public async Task<AddMessageResult> Send(AddMessageRequest addMessageRequest, string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                if (!_validator.IsMessageRequestValid(addMessageRequest, nhsLoginId))
                {
                    return new AddMessageResult.BadRequest();
                }

                var userMessage = _addMessageToUserMessageMapper.Map(addMessageRequest, nhsLoginId);
                var result = await _messageRepository.Create(userMessage);

                return result.Accept(new RepositoryCreateResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Message Posting has failed with exception");
                return new AddMessageResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<MessagesResult> GetMessage(AccessToken accessToken, string messageId)
        {
            _logger.LogEnter();

            try
            {
                if (!_validator.IsMessageIdValid(messageId))
                {
                    return new MessagesResult.BadRequest();
                }

                var result = await _messageRepository.FindMessage(accessToken.Subject, messageId);

                return result.Accept(new RepositoryFindMessageResultVisitor(_userMessageToResponseMapper));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Message Get has failed with exception");
                return new MessagesResult.InternalServerError();
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

                return result.Accept(new RepositoryFindMessagesResultVisitor(_userMessagesToResponseMapper));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Sender Messages Get has failed with exception");
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
                    new RepositoryFindSummaryMessagesResultVisitor(_summaryMessagesToResponseMapper));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Summary Messages Get has failed with exception");
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

                var repositoryUpdates = new UpdateMessageMapperStep(_logger).Map(messagePatchDocument);
                if (repositoryUpdates.Failed(out var repositoryUpdatesFailure))
                {
                    return repositoryUpdatesFailure;
                }

                var updateResult =
                    await _messageRepository.UpdateOne(accessToken.Subject, messageId, repositoryUpdates);
                if (!(updateResult is RepositoryUpdateResult<UserMessage>.Updated))
                {
                    _logger.LogWarning("Message Patch did not update message in repository");
                    return updateResult.Accept(new RepositoryUpdateMessageResultVisitor());
                }

                var findResult = await _messageRepository.FindMessage(accessToken.Subject, messageId);
                if (!(findResult is RepositoryFindResult<UserMessage>.Found foundResult))
                {
                    _logger.LogWarning("Message Patch failed to find message in repository");
                    return findResult.Accept(new RepositoryFindMessagePatchResultVisitor());
                }

                var record = foundResult.Records.First();

                return new MessagePatchResult.Updated(record);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Message Patch has failed with exception");
                return new MessagePatchResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<SendersResult> GetSenders(AccessToken accessToken)
        {
            _logger.LogEnter();

            try
            {
                var result = await _messageRepository.FindAllForUser(accessToken.Subject);

                return result.Accept(new RepositoryFindSendersResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get Senders has failed with exception");
                return new SendersResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
