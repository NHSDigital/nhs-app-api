using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auditing
{
    public interface IAuditBuilder : IAuditBuilderAccessToken, IAuditBuilderNhsNumber, IAuditBuilderOperation
    {
    }

    public interface IAuditBuilderAccessToken
    {
        IAuditBuilderNhsNumber AccessToken(string accessToken);
        IAuditBuilderNhsNumber AccessToken(AccessToken accessToken);
    }

    public interface IAuditBuilderNhsNumber
    {
        IAuditBuilderSupplier NhsNumber(string nhsNumber);
    }

    public interface IAuditBuilderSupplier
    {
        IAuditBuilderOperation Supplier(Supplier supplier);
    }

    public interface IAuditBuilderOperation
    {
        IAuditBuilderDetails Operation(string operation);
    }

    public interface IAuditBuilderDetails
    {
        IAuditBuilderExecute Details(string details);
        IAuditBuilderExecute Details(string details, params object[] parameters);
    }

    public interface IAuditBuilderExecute
    {
        Task<TAuditedResult> Execute<TAuditedResult>(Func<Task<TAuditedResult>> auditedAction) where TAuditedResult : IAuditedResult;
    }
}