using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auditing.UnitTestsSupport
{
    public sealed class AuditBuilderStub: IAuditBuilder, IAuditBuilderSupplier, IAuditBuilderDetails, IAuditBuilderExecute
    {
        public string AccessTokenString { get; set; }
        public AccessToken AccessToken { get; set; }
        public string NhsNumber { get; set; }
        public Supplier Supplier { get; set; }
        public string Operation { get; set; }
        public string Details { get; set; }
        public List<object> Parameters { get; set; }
        public string ResponseDetails { get; set; }

        IAuditBuilderNhsNumber IAuditBuilderAccessToken.AccessToken(string accessToken)
        {
            AccessTokenString = accessToken;
            return this;
        }

        IAuditBuilderNhsNumber IAuditBuilderAccessToken.AccessToken(AccessToken accessToken)
        {
            AccessToken = accessToken;
            return this;
        }

        IAuditBuilderSupplier IAuditBuilderNhsNumber.NhsNumber(string nhsNumber)
        {
            NhsNumber = nhsNumber;
            return this;
        }

        IAuditBuilderOperation IAuditBuilderSupplier.Supplier(Supplier supplier)
        {
            Supplier = supplier;
            return this;
        }

        IAuditBuilderDetails IAuditBuilderOperation.Operation(string operation)
        {
            Operation = operation;
            return this;
        }

        IAuditBuilderExecute IAuditBuilderDetails.Details(string details)
        {
            Details = details;
            return this;
        }

        IAuditBuilderExecute IAuditBuilderDetails.Details(string details, params object[] parameters)
        {
            Details = details;
            Parameters = parameters.ToList();
            return this;
        }

        async Task<TAuditedResult> IAuditBuilderExecute.Execute<TAuditedResult>(Func<Task<TAuditedResult>> auditedAction)
        {
            var result = await  auditedAction();
            ResponseDetails = result.Details;
            return result;
        }
    }
}