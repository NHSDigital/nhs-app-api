using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.Messages.Areas.Messages.Mappers
{
    public class SendersResponseMapper :
        IMapper<List<DbSender>, SendersResponse>,
        IMapper<DbSender, SendersResponse>
    {
        private readonly ILogger<SendersResponseMapper> _logger;

        public SendersResponseMapper(ILogger<SendersResponseMapper> logger)
        {
            _logger = logger;
        }

        public SendersResponse Map(DbSender source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsValid();

            return new SendersResponse
            {
                Senders = new List<Sender>
                {
                    new Sender
                    {
                        Name = source.Name,
                        Id = source.Id
                    }
                }
            };
        }

        public SendersResponse Map(List<DbSender> source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsValid();

            if (source.Any())
            {
                return new SendersResponse
                {
                    Senders =
                        source.Select(x => new Sender
                        {
                            Id = x.Id,
                            Name = x.Name
                        }).ToList()
                };
            }

            return new SendersResponse();
        }
    }
}