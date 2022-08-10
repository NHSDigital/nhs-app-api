using System.Collections.Generic;
using NHSOnline.HttpMocks.SecondaryCare.Builders;
using OperationOutcome = Hl7.Fhir.Model.OperationOutcome;

namespace NHSOnline.HttpMocks.SecondaryCare
{
    public static class StubbedPartialErrorResponses
    {
        public static Dictionary<string, IList<OperationOutcome.IssueComponent>> PartialErrorMapping { get; } =
            new()
            {
                {"9186712039", new []{ StructurePartialError() }},
                {"9461823282", new []{ TransientPartialError(), TimeoutPartialError() }},
                {"2090220899", new []{ ForbiddenPartialError() }},
            };

        private static OperationOutcome.IssueComponent TimeoutPartialError() =>
            new PartialErrorBuilder()
                .WithIssueType(OperationOutcome.IssueType.Timeout)
                .WithDescription("Timeout occured")
                .WithProviderId("company-3")
                .Build();

        private static OperationOutcome.IssueComponent TransientPartialError() =>
            new PartialErrorBuilder()
                .WithIssueType(OperationOutcome.IssueType.Transient)
                .WithDescription("http-404 returned")
                .WithProviderId("company-2")
                .Build();

        private static OperationOutcome.IssueComponent StructurePartialError() =>
            new PartialErrorBuilder()
                .WithIssueType(OperationOutcome.IssueType.Structure)
                .WithDescription("Response failed validation")
                .WithProviderId("company-1")
                .Build();

        private static OperationOutcome.IssueComponent ForbiddenPartialError() =>
            new PartialErrorBuilder()
                .UnderAge()
                .Build();
    }
}