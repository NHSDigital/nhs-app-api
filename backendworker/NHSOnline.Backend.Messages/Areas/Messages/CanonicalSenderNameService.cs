using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using static System.String;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    public class CanonicalSenderNameService : ICanonicalSenderNameService
    {
        private class SenderIdDerivation
        {
            public string SupplierId { get; set; }
            public Regex SenderName { get; set; }
            public string SenderId { get; set; }
        }

        private const string MJogSupplierId = "409a0887-a946-4884-9796-45296a053192";
        private string NhsAppSupplierId => _config.SupplierIdNhsApp;

        private IEnumerable<SenderIdDerivation> SenderIdLookup => new List<SenderIdDerivation>
        {
            new SenderIdDerivation
            {
                SupplierId = MJogSupplierId, SenderName = new Regex("Trinity Medical Centre"),
                SenderId = "G81070"
            },
            new SenderIdDerivation
            {
                SupplierId = MJogSupplierId, SenderName = new Regex("Parliament Street Medical Centre"),
                SenderId = "Y02847"
            },
            new SenderIdDerivation
            {
                SupplierId = MJogSupplierId, SenderName = new Regex("Grange Farm Medical Centre"),
                SenderId = "Y03124"
            },
            new SenderIdDerivation
            {
                SupplierId = MJogSupplierId, SenderName = new Regex("Deer Park Family Medical Practice"),
                SenderId = "C84044"
            },
            new SenderIdDerivation
            {
                SupplierId = MJogSupplierId, SenderName = new Regex("Bilborough M(edical )?C(entre)?"),
                SenderId = "Y06356"
            },
            new SenderIdDerivation
            {
                SupplierId = MJogSupplierId, SenderName = new Regex("Assarts Farm"),
                SenderId = "NO083"
            },
            new SenderIdDerivation
            {
                SupplierId = MJogSupplierId, SenderName = new Regex("Aspley Medical Centre"),
                SenderId = "C84091"
            },
            new SenderIdDerivation
            {
                SupplierId = MJogSupplierId, SenderName = new Regex("Aldborough Surgery"),
                SenderId = "D82628"
            },
            new SenderIdDerivation
            {
                SupplierId = NhsAppSupplierId, SenderName = new Regex("Middleton Lodge Practice"),
                SenderId = "C84021"
            }
        };

        private readonly ISenderService _senderService;
        private readonly IMessagesConfiguration _config;

        public CanonicalSenderNameService(
            ISenderService senderService,
            IMessagesConfiguration config)
        {
            _senderService = senderService;
            _config = config;
        }

        public async Task UpdateWithCanonicalSenderName(ICollection<UserMessage> messages)
        {
            if (_config.SenderIdEnabled)
            {
                foreach (var message in messages)
                {
                    await UpdateUserMessageWithCanonicalSenderName(message);
                }
            }
        }

        private async Task UpdateUserMessageWithCanonicalSenderName(UserMessage message)
        {
            message.SenderContext ??= new SenderContext();
            message.SenderContext.SupplierId ??= _config.SupplierIdNhsApp;
            message.SenderContext.SenderId ??= GetSenderId(message);

            var sendersResult = await _senderService.GetSender(message.SenderContext.SenderId);
            var senderName = (sendersResult as SendersResult.Found)?.Response.Senders.FirstOrDefault()?.Name;

            message.Sender = senderName ?? message.Sender;
        }

        private string GetSenderId(UserMessage message)
        {
            return SenderIdLookup
                .FirstOrDefault(x =>
                    x.SupplierId == message.SenderContext.SupplierId
                    && x.SenderName.IsMatch(message.Sender ?? Empty))
                ?.SenderId ?? _config.SenderIdNhsApp;
        }
    }
}