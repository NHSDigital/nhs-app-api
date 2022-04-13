using System.Linq;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    internal class RepositoryFindMessageResultVisitor : IRepositoryFindResultVisitor<UserMessage, MessagesResult>
    {
        private readonly IMapper<UserMessage, MessagesResponse> _mapper;

        public RepositoryFindMessageResultVisitor(IMapper<UserMessage, MessagesResponse> mapper)
        {
            _mapper = mapper;
        }

        public MessagesResult Visit(RepositoryFindResult<UserMessage>.NotFound result)
        {
            return new MessagesResult.None();
        }

        public MessagesResult Visit(RepositoryFindResult<UserMessage>.RepositoryError result)
        {
            return new MessagesResult.BadGateway();
        }

        public MessagesResult Visit(RepositoryFindResult<UserMessage>.Found result)
        {
            var response = _mapper.Map(result.Records.Single());

            if (response.SenderMessages.Any())
            {
                return new MessagesResult.Found(response);
            }

            return new MessagesResult.None();
        }
    }
}