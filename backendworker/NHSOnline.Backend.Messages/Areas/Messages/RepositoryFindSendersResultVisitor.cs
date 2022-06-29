using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    internal class RepositoryFindSendersResultVisitor : IRepositoryFindResultVisitor<DbSender, SendersResult>
    {
        private readonly IMapper<List<DbSender>, SendersResponse> _mapper;

        public RepositoryFindSendersResultVisitor(IMapper<List<DbSender>, SendersResponse> mapper)
        {
            _mapper = mapper;
        }

        public SendersResult Visit(RepositoryFindResult<DbSender>.NotFound result)
        {
            return new SendersResult.None();
        }

        public SendersResult Visit(RepositoryFindResult<DbSender>.Found result)
        {
            var response = _mapper.Map(result.Records.ToList());

            if (response.Senders.Any())
            {
                return new SendersResult.Found(response);
            }

            return new SendersResult.None();
        }

        public SendersResult Visit(RepositoryFindResult<DbSender>.RepositoryError result)
        {
            return new SendersResult.BadGateway();
        }
    }
}