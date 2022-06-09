using System.Linq;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    internal class RepositoryFindSenderResultVisitor : IRepositoryFindResultVisitor<DbSender, SenderResult>
    {
        private readonly IMapper<DbSender, Sender> _mapper;

        public RepositoryFindSenderResultVisitor(IMapper<DbSender, Sender> mapper)
        {
            _mapper = mapper;
        }

        public SenderResult Visit(RepositoryFindResult<DbSender>.NotFound result)
        {
            return new SenderResult.NotFound();
        }

        public SenderResult Visit(RepositoryFindResult<DbSender>.Found result)
        {
            var response = _mapper.Map(result.Records.Single());

            if (response != null)
            {
                return new SenderResult.Found(response);
            }

            return new SenderResult.NotFound();
        }

        public SenderResult Visit(RepositoryFindResult<DbSender>.RepositoryError result)
        {
            return new SenderResult.BadGateway();
        }
    }
}