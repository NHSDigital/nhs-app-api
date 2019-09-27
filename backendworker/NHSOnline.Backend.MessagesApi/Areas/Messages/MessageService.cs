using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    internal class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<MessageController> _logger;

        public MessageService
        (
            IMessageRepository messageRepository,
            ILogger<MessageController> logger
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
                    Body = addMessageRequest.Body
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
    }
}