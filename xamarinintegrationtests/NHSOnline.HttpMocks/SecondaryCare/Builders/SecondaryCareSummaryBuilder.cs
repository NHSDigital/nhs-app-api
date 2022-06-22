using System.Collections.Generic;
using Hl7.Fhir.Model;
using CarePlanIntent = Hl7.Fhir.Model.CarePlan.CarePlanIntent;
using OperationOutcome = Hl7.Fhir.Model.OperationOutcome;

namespace NHSOnline.HttpMocks.SecondaryCare.Builders
{
    public class SecondaryCareSummaryBuilder
    {
        private string _nhsNumber = string.Empty;
        private List<OperationOutcome.IssueComponent> _errors = new List<OperationOutcome.IssueComponent>();
        private List<CarePlan.ActivityComponent> _referralsAndAppointments = new List<CarePlan.ActivityComponent>();

        public SecondaryCareSummaryBuilder ForNhsNumber(string nhsNumber)
        {
            _nhsNumber = nhsNumber;
            return this;
        }

        public SecondaryCareSummaryBuilder WithReferrals(IEnumerable<CarePlan.ActivityComponent> referrals)
            => AddActivities(referrals);

        public SecondaryCareSummaryBuilder WithUpcomingAppointments(IEnumerable<CarePlan.ActivityComponent> upcomingAppointments)
            => AddActivities(upcomingAppointments);

        public SecondaryCareSummaryBuilder WithPartialErrors(IEnumerable<OperationOutcome.IssueComponent> errors)
        {
            _errors ??= new List<OperationOutcome.IssueComponent>();
            _errors.AddRange(errors);
            return this;
        }

        private SecondaryCareSummaryBuilder AddActivities(IEnumerable<CarePlan.ActivityComponent> activities)
        {
            _referralsAndAppointments ??= new List<CarePlan.ActivityComponent>();
            _referralsAndAppointments.AddRange(activities);

            return this;
        }

        private Bundle.EntryComponent GenerateCarePlan() =>
            new()
            {
                FullUrl = "https://fake-sandbox.co.uk/CarePlan/1",
                Search = new Bundle.SearchComponent
                {
                    Mode = Bundle.SearchEntryMode.Match
                },
                Resource = new CarePlan
                {
                    Activity = _referralsAndAppointments,
                    Status = RequestStatus.Active,
                    Intent = CarePlanIntent.Order,
                    Subject = FhirBuilderHelpers.GetNhsNumberReference(_nhsNumber),
                }
            };


        public Bundle Build()
        {
            var bundle = new Bundle
            {
                Entry = new List<Bundle.EntryComponent> { GenerateCarePlan() }
            };

            if (_errors.Count > 0)
            {
                bundle.Entry.Add(
                    new Bundle.EntryComponent
                    {
                        FullUrl = "https://fake-sandbox.co.uk/OperationOutCome/1",
                        Search = new Bundle.SearchComponent
                        {
                            Mode = Bundle.SearchEntryMode.Include
                        },
                        Resource = new OperationOutcome
                        {
                            Issue = _errors
                        }
                    });
            }

            return bundle;
        }
    }
}