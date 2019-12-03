using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.GpSystems.LinkedAccounts.Models
{
    public class LinkedAccountsConfigResponse
    {
        public Guid Id { get; set; }

        public bool HasLinkedAccounts { get; set; }

        public IEnumerable<LinkedAccount> LinkedAccounts { get; set; } = Enumerable.Empty<LinkedAccount>();
    }
}
