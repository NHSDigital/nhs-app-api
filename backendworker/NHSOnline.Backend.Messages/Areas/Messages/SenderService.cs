using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Cache.Messages;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class SenderService : ISenderService
    {
        private readonly ISenderRepository _senderRepository;
        private readonly ISenderCacheProvider _cacheProvider;
        private readonly ILogger<SenderService> _logger;
        private readonly IMapper<List<DbSender>, SendersResponse> _sendersResponseMapper;
        private readonly IMapper<Sender, DbSender> _senderRequestMapper;
        private readonly IMapper<DbSender, SendersResponse> _senderResponseMapper;

        public SenderService
        (
            ISenderRepository senderRepository,
            ISenderCacheProvider cacheProvider,
            IMapper<Sender, DbSender> senderRequestMapper,
            IMapper<List<DbSender>, SendersResponse> sendersResponseMapper,
            IMapper<DbSender, SendersResponse> senderResponseMapper,
            ILogger<SenderService> logger)
        {
            _senderRepository = senderRepository;
            _cacheProvider = cacheProvider;
            _logger = logger;
            _sendersResponseMapper = sendersResponseMapper;
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

        public async Task<SendersResult> GetSender(string senderId)
        {
            try
            {
                _logger.LogEnter();

                senderId = senderId.ToUpperInvariant();

                var sender = _cacheProvider.GetSender(senderId);

                if (sender != null)
                {
                    _logger.LogInformation("Found sender with id {senderId} in cache", senderId);
                    return new SendersResult.Found(new SendersResponse
                    {
                        Senders = new List<Sender> { sender }
                    });
                }

                return await HandleInMemoryCacheMiss(senderId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get Sender has failed with exception");

                return new SendersResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<SendersResult> GetSenders(DateTime lastUpdatedBefore, int limit)
        {
            try
            {
                _logger.LogEnter();

                var result = await _senderRepository.Find(lastUpdatedBefore, limit);

                return result.Accept(new RepositoryFindSendersResultVisitor(_sendersResponseMapper));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get Sender has failed with exception");

                return new SendersResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
        private async Task<SendersResult> HandleInMemoryCacheMiss(string senderId)
        {
            _logger.LogInformation("Unable to find sender with id {senderId} in cache", senderId);
            var repositoryFindResult = await _senderRepository.Find(senderId);

            var sendersResult = repositoryFindResult.Accept(new RepositoryFindSenderResultVisitor(_senderResponseMapper));

            if (sendersResult is SendersResult.Found foundResult)
            {
                _cacheProvider.SetSender(foundResult.Response.Senders.Single());
                _logger.LogInformation("Found sender with id {senderId} in senders container after cache miss, added to cache", senderId);
            }

            return sendersResult;
        }
    }
}