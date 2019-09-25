using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using NHSOnline.Backend.Auth.CitizenId.Models;
using MongoDB.Driver;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    internal class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<MessagesController> _logger;

        public MessageService
        (
            IMessageRepository messageRepository,
            ILogger<MessagesController> logger
        )
        {
            _messageRepository = messageRepository;
            _logger = logger;
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

        public async Task<MessagesResult> GetMessages(AccessToken accessToken)
        {
            _logger.LogEnter();

            try
            {
                var messages = await _messageRepository.Find(accessToken.Subject);
                if (messages.Any())
                {
                    return new MessagesResult.Some(messages);
                }

                return new MessagesResult.None();
            }
            catch (MongoException e)
            {
                _logger.LogError($"Message Get has failed with exception: {e}");
                return new MessagesResult.BadGateway();
            }
            catch (Exception e)
            {
                _logger.LogError($"Message Get has failed with exception: {e}");
                return new MessagesResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}