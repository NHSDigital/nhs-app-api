using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using OperationOutcome = Hl7.Fhir.Model.OperationOutcome;

namespace NHSOnline.HttpMocks.SecondaryCare.Builders
{
    public class PartialErrorBuilder
    {
        private OperationOutcome.IssueType? _issueType;
        private string? _description;
        private string? _providerId;

        public PartialErrorBuilder WithIssueType(OperationOutcome.IssueType issueType)
        {
            _issueType = issueType;
            return this;
        }

        public PartialErrorBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public PartialErrorBuilder WithProviderId(string providerId)
        {
            _providerId = providerId;
            return this;
        }

        public PartialErrorBuilder UnderAge()
        {
            _description = "UNDER_16_DENIED";
            _issueType = OperationOutcome.IssueType.Forbidden;
            return this;
        }

        public OperationOutcome.IssueComponent Build()
        {
            if (_issueType is null)
            {
                throw new InvalidOperationException("Issue type has not been provided. User build method `WithIssueType`");
            }

            if (string.IsNullOrEmpty(_description))
            {
                throw new InvalidOperationException("Description has not been provided. User build method `WithDescription`");
            }

            if (string.IsNullOrEmpty(_providerId) && _issueType != OperationOutcome.IssueType.Forbidden)
            {
                throw new InvalidOperationException("ProviderId has not been provided. User build method `WithProviderId`");
            }

            var issueComponent = new OperationOutcome.IssueComponent
            {
                Severity = OperationOutcome.IssueSeverity.Error,
                Code = _issueType,
                Diagnostics = _description,
                Extension = new List<Extension>
                {
                    FhirBuilderHelpers.GetErrorExtension(_providerId!),
                },
            };

            return issueComponent;
        }
    }
}