using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.LinkedAccounts
{
    public interface ITppLinkedAccountsService
    {
        List<Person> ExtractValidProxyPatients(AuthenticateReply response);

        void LogMismatchingPractices(AuthenticateReply authenticateReply, ICollection<TppProxyUserSession> proxyPatients);
    }
}
