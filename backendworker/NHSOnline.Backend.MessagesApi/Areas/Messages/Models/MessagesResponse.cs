using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    [SuppressMessage("Microsoft.Naming", "CA1710", Justification = "Naming consistent with endpoint response")]
    public class MessagesResponse : List<SenderMessages>
    {
        public MessagesResponse()
        {
        }

        public MessagesResponse(IEnumerable<SenderMessages> collection)
            : base(collection)
        {
        }
    }
}