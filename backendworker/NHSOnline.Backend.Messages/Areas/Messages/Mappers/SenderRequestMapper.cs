using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Messages.Areas.Messages.Mappers
{
    public class SenderRequestMapper : IMapper<Sender, DbSender>
    {
        public DbSender Map(Sender source)
        {
            if (source == null)
            {
                return new DbSender();
            }

            return new DbSender
            {
                Id = source.Id.ToUpperInvariant(),
                Name = source.Name
            };
        }
    }
}