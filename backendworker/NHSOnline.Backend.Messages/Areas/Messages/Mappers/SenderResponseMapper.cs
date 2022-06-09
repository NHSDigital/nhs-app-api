using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Messages.Areas.Messages.Mappers
{
    public class SenderResponseMapper : IMapper<DbSender, Sender>
    {
        public Sender Map(DbSender source)
        {
            if (source == null)
            {
                return new Sender();
            }

            return new Sender
            {
                Name = source.Name,
                Id = source.Id
            };
        }
    }
}