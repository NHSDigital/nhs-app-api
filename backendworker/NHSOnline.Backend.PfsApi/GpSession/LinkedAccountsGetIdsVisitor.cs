using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.LinkedAccounts;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public class LinkedAccountsGetIdsVisitor : ILinkedAccountsResultVisitor<IList<Guid>>
    {
        public IList<Guid> Visit(LinkedAccountsResult.Success result)
        {
            return result.ValidAccounts
                .Select(a => a.Id)
                .ToList();
        }
    }
}