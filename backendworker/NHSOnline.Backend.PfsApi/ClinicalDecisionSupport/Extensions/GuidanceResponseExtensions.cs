using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Extensions
{
    public static class GuidanceResponseExtensions
    {
        public static List<OperationOutcome> ExtractOperationOutcomes(this DomainResource guidanceResponse)
        {
            var operationOutcomes = new List<OperationOutcome>();

            if (guidanceResponse.Contained == null)
            {
                return operationOutcomes;
            }

            foreach (var container in guidanceResponse.Contained
                .Where(a => a.ResourceType == ResourceType.OperationOutcome))
            {
                operationOutcomes.Add((OperationOutcome)container);
            }

            return operationOutcomes;
        }

        public static List<Coding> ExtractNotFoundOutcomes(this List<OperationOutcome> outcomes)
        {
            var notFoundOutcomes = new List<Coding>();
            foreach (var issue in outcomes
                .SelectMany(outcome => outcome.Issue.Where(a => a.Code == OperationOutcome.IssueType.NotFound)))
            {
                notFoundOutcomes.AddRange(issue.Details.Coding);
            }

            return notFoundOutcomes;
        }

        public static Boolean IsSessionEnded(this List<Coding> codings) => 
            codings.Any(code => string.Equals(code.Code, Constants.IssueCodes.SessionEnd, StringComparison.Ordinal));
    }
}