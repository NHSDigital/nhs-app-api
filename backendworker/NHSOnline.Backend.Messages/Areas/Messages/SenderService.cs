using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class SenderService : ISenderService
    {
        private readonly ISenderRepository _senderRepository;
        private readonly ILogger<SenderService> _logger;
        private readonly IMapper<Sender, DbSender> _senderRequestMapper;
        private readonly IMapper<DbSender, Sender> _senderResponseMapper;

        public SenderService
        (
            ISenderRepository senderRepository,
            ILogger<SenderService> logger,
            IMapper<Sender, DbSender> senderRequestMapper,
            IMapper<DbSender, Sender> senderResponseMapper)
        {
            _senderRepository = senderRepository;
            _logger = logger;
            _senderRequestMapper = senderRequestMapper;
            _senderResponseMapper = senderResponseMapper;
        }

        public async Task<SenderPostResult> Create(Sender sender)
        {
            try
            {
                _logger.LogEnter();

                var addSender = _senderRequestMapper.Map(sender);
                var result = await _senderRepository.CreateOrUpdate(addSender);

                return result.Accept(new RepositorySenderPostResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Sender Posting has failed with exception");

                return new SenderPostResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<SenderResult> GetSender(string senderId)
        {
            try
            {
                _logger.LogEnter();

                var result = await _senderRepository.Find(senderId.ToUpperInvariant());

                return result.Accept(new RepositoryFindSenderResultVisitor(_senderResponseMapper));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get Sender has failed with exception");

                return new SenderResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}