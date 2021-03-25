using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessageLinkClickedService : IMessageLinkClickedService
    {
        private readonly ILogger<MessageLinkClickedService> _logger;
        private readonly IMapper<MessageLink, RepositoryFindResult<UserMessage>.Found, MessageLinkClickedData> _mapper;
        private readonly IMessageLinkClickedValidationService _validationService;
        private readonly IMessageRepository _messageRepository;
        private readonly IMetricLogger _metricLogger;

        public MessageLinkClickedService(
            ILogger<MessageLinkClickedService> logger,
            IMapper<MessageLink, RepositoryFindResult<UserMessage>.Found, MessageLinkClickedData> mapper,
            IMessageLinkClickedValidationService validationService,
            IMessageRepository messageRepository,
            IMetricLogger metricLogger
        )
        {
            _logger = logger;
            _mapper = mapper;
            _validationService = validationService;
            _messageRepository = messageRepository;
            _metricLogger = metricLogger;
        }

        public async Task<MessageLinkClickedResult> LogLinkClicked(string nhsLoginId, MessageLink messageLink)
        {
            try
            {
                _logger.LogEnter();

                if (!_validationService.IsServiceRequestValid(nhsLoginId, messageLink))
                {
                    return new MessageLinkClickedResult.BadRequest();
                }

                var result = await _messageRepository.FindMessage(nhsLoginId, messageLink.MessageId);

                if (!(result is RepositoryFindResult<UserMessage>.Found found))
                {
                    return result.Accept(new RepositoryFindMessageLinkClickedResultVisitor());
                }

                var data = _mapper.Map(messageLink, found);

                await _metricLogger.MessageLinkClicked(data);

                return new MessageLinkClickedResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $@"Failed to log metrics for {messageLink}");
                return new MessageLinkClickedResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
