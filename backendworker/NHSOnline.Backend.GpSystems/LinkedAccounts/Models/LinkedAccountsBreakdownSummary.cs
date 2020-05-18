using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts.Models
{
    public class LinkedAccountsBreakdownSummary
    {
        public IEnumerable<LinkedAccount> ValidAccounts { get; set; } = Enumerable.Empty<LinkedAccount>();

        public IEnumerable<LinkedAccount> AccountsWithNoNhsNumber { get; set; } = Enumerable.Empty<LinkedAccount>();
    }
}
