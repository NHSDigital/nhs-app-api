using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
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

        public MessageService
        (
            IMessageRepository messageRepository,
            ILogger<MessagesController> logger,
            IMapper<List<UserMessage>, MessagesResponse> userMessagesToResponseMapper,
            IMapper<List<SummaryMessage>, MessagesResponse> summaryMessagesToResponseMapper
        )
        {
            _messageRepository = messageRepository;
            _logger = logger;
            _userMessagesToResponseMapper = userMessagesToResponseMapper;
            _summaryMessagesToResponseMapper = summaryMessagesToResponseMapper;
        }

        public async Task<MessageResult> Send(AddMessageRequest addMessageRequest, string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();
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