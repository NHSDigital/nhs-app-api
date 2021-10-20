using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    internal class RepositoryFindMessagesResultVisitor : IRepositoryFindResultVisitor<UserMessage, MessagesResult>
    {
        private readonly IMapper<List<UserMessage>, MessagesResponse> _mapper;

        public RepositoryFindMessagesResultVisitor(IMapper<List<UserMessage>, MessagesResponse> mapper)
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
            var response = _mapper.Map(result.Records.ToList());

            if (response.SenderMessages.Any())
            {
                return new MessagesResult.Found(response);
            }

            return new MessagesResult.None();
        }
    }
}