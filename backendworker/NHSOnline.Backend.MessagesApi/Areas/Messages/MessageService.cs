using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using MongoDB.Driver;
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
        private readonly IMapper<JsonPatchDocument<Message>, JsonPatchDocument<UserMessage>> _messageToUserMessagePatchMapper;
        private readonly IMessagesValidationService _validator;

        
        private static readonly Dictionary<string,List<OperationType>> OperationsWhiteList = 
            new Dictionary<string, List<OperationType>>
            {
                {"/READ", new List<OperationType>{OperationType.Add}}
            };
        
        public MessageService
        (
            IMessageRepository messageRepository,
            ILogger<MessagesController> logger,
            IMapper<List<UserMessage>, MessagesResponse> userMessagesToResponseMapper,
            IMapper<List<SummaryMessage>, MessagesResponse> summaryMessagesToResponseMapper,
            IMapper<JsonPatchDocument<Message>, JsonPatchDocument<UserMessage>> messageToUserMessagePatchMapper, 
            IMessagesValidationService validator)
        {
            _messageRepository = messageRepository;
            _logger = logger;
            _userMessagesToResponseMapper = userMessagesToResponseMapper;
            _summaryMessagesToResponseMapper = summaryMessagesToResponseMapper;
            _messageToUserMessagePatchMapper = messageToUserMessagePatchMapper;
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
                    SentTime = DateTime.Now
                };

                await _messageRepository.Create(userMessage);
               
                return new MessageResult.Success();
            }
            catch (MongoException e)
            {
                _logger.LogError($"Message Posting has failed with exception: {e}");
                return new MessageResult.BadGateway();
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
                var messages = await _messageRepository.Find(accessToken.Subject, sender);

                return MapResult(_userMessagesToResponseMapper, messages);
            }
            catch (MongoException e)
            {
                _logger.LogError($"Sender Messages Get has failed with exception: {e}");
                return new MessagesResult.BadGateway();
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
                var summaryMessages = await _messageRepository.Summary(accessToken.Subject);

                return MapResult(_summaryMessagesToResponseMapper, summaryMessages);
            }
            catch (MongoException e)
            {
                _logger.LogError($"Summary Messages Get has failed with exception: {e}");
                return new MessagesResult.BadGateway();
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

        public async Task<MessagePatchResult> PatchMessage(JsonPatchDocument<Message> messagePatchDocument, string messageId)
        {
            _logger.LogEnter();

            try
            { 
                if (!_validator.IsPatchRequestValid(messagePatchDocument, messageId)
                || !ValidatePatchOperations(messagePatchDocument.Operations))
                {
                    return new MessagePatchResult.BadRequest();
                }
                
                var userMessage = await _messageRepository.FindOne(messageId);
                if (userMessage != null)
                {
                    var userMessagePatchDocument = _messageToUserMessagePatchMapper.Map(messagePatchDocument);
                    
                    userMessagePatchDocument.ApplyTo(userMessage);
                    
                    await _messageRepository.UpdateOne(userMessage);
                    return new MessagePatchResult.Updated();
                }
                return new MessagePatchResult.NotFound();
            } 
            catch (MongoException e)
            {
                _logger.LogError($"Message Get has failed with exception: {e}");
                return new MessagePatchResult.BadGateway(); 
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

        private static bool ValidatePatchOperations(List<Operation<Message>> operations)
        {
            foreach (var operation in operations)
            {
                var path = operation.path.ToUpperInvariant();
                if (!OperationsWhiteList.ContainsKey(path))
                {
                    return false;
                }
                if (!OperationsWhiteList[path].Contains(operation.OperationType))
                {
                    return false;
                }
            }
            return true;
        }

        private static MessagesResult MapResult<TSource>(IMapper<TSource, MessagesResponse> mapper, TSource source)
        {
            var response = mapper.Map(source);

            if (response.Any())
            {
                return new MessagesResult.Some(response);
            }

            return new MessagesResult.None();
        }
    }
}