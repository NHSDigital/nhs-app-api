using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts.Models
{
    public class GetLinkedAccountsResponse
    {
        public IEnumerable<LinkedAccount> LinkedAccounts { get; set; } = Enumerable.Empty<LinkedAccount>();
    }
}
