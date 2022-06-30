using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Cache.Messages;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class SenderService : ISenderService
    {
        private readonly ISenderRepository _senderRepository;
        private readonly ISenderCacheProvider _cacheProvider;
        private readonly IMapper<Sender, DbSender> _senderRequestMapper;
        private readonly IMapper<DbSender, Sender> _senderResponseMapper;
        private readonly ILogger<SenderService> _logger;

        public SenderService
        (
            ISenderRepository senderRepository,
            ISenderCacheProvider cacheProvider,
            IMapper<Sender, DbSender> senderRequestMapper,
            IMapper<DbSender, Sender> senderResponseMapper,
            ILogger<SenderService> logger)
        {
            _senderRepository = senderRepository;
            _cacheProvider = cacheProvider;
            _senderRequestMapper = senderRequestMapper;
            _senderResponseMapper = senderResponseMapper;
            _logger = logger;

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

                senderId = senderId.ToUpperInvariant();

                var sender = _cacheProvider.GetSender(senderId);

                var result = sender switch
                {
                    null => await HandleInMemoryCacheMiss(senderId),
                    _ => LogSenderAndReturn(sender)
                };

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


        private RepositoryFindResult<DbSender> LogSenderAndReturn(Sender sender)
        {
            _logger.LogInformation($"Retrieved sender name {sender.Name} from cache");

            return new RepositoryFindResult<DbSender>.Found(
                new List<DbSender>
                {
                    _senderRequestMapper.Map(sender)
                });
        }

        private async Task<RepositoryFindResult<DbSender>> HandleInMemoryCacheMiss(string senderId)
        {
            var findResult = await _senderRepository.Find(senderId);

            if (findResult is RepositoryFindResult<DbSender>.Found foundResult)
            {
                var dbSender = foundResult.Records.First();
                var sender = _senderResponseMapper.Map(dbSender);
                UpdateSenderCache(sender);
            }
            return findResult;
        }

        private Sender UpdateSenderCache(Sender sender)
        {
            _cacheProvider.SetSender(sender);
            _logger.LogInformation($"Added sender name {sender.Name} from repository to cache");
            return sender;
        }
    }
}