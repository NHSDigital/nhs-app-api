using System.Collections.Generic;
using NHSOnline.HttpMocks.SecondaryCare.Builders;
using Hl7.Fhir.Model;

namespace NHSOnline.HttpMocks.SecondaryCare
{
    public static class StubbedReferralResponses
    {
        public static Dictionary<string, IList<CarePlan.ActivityComponent>> ReferralMapping { get; } =
            new()
            {
                {"9414105131", new List<CarePlan.ActivityComponent>()},
                {
                    "9414105132", new List<CarePlan.ActivityComponent>()
                    {
                        ReferralInReviewDue(),
                        ReferralBookable(),
                        ReferralBookableNoSpecialty(),
                        ReferralInReviewDueNoSpecialty(),
                        ReferralBookableWasCancelled(),
                        ReferralBookableWasCancelledNoSpecialty(),
                    }
                },
            };

        private static CarePlan.ActivityComponent ReferralInReview() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.inReview)
                .WithReferredDateDaysAgo(4)
                .WithSpecialty(ServiceSpecialty.General)
                .WithPerformerOrganisation(ReferrerOrganisation.Mahogany)
                .Build();

        private static CarePlan.ActivityComponent ReferralInReviewPkb() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.inReview)
                .WithReferredDateDaysAgo(3)
                .WithSpecialty(ServiceSpecialty.General)
                .WithPerformerOrganisation(ReferrerOrganisation.Birch)
                .WithProvider(ServiceProvider.PKB)
                .Build();

        private static CarePlan.ActivityComponent ReferralInReviewDue() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.inReview)
                .WithReferredDateDaysAgo(8)
                .WithDueDateInDays(5)
                .WithSpecialty(ServiceSpecialty.Cardiology)
                .WithPerformerOrganisation(ReferrerOrganisation.Willow)
                .Build();

        private static CarePlan.ActivityComponent ReferralInReviewDueNoSpecialty() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.inReview)
                .WithReferredDateDaysAgo(9)
                .WithSpecialty(ServiceSpecialty.None)
                .WithDueDateInDays(5)
                .Build();

        private static CarePlan.ActivityComponent ReferralInReviewOverdue() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.inReview)
                .WithReferredDateDaysAgo(5)
                .WithMissedDueDateDaysAgo(2)
                .WithSpecialty(ServiceSpecialty.Haematology)
                .WithPerformerOrganisation(ReferrerOrganisation.Oak)
                .Build();

        private static CarePlan.ActivityComponent ReferralInReviewOverdueNoSpecialty() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.inReview)
                .WithReferredDateDaysAgo(5)
                .WithMissedDueDateDaysAgo(2)
                .WithSpecialty(ServiceSpecialty.None)
                .WithPerformerOrganisation(ReferrerOrganisation.Oak)
                .Build();

        private static CarePlan.ActivityComponent ReferralBookable() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.bookable)
                .WithReferredDateDaysAgo(16)
                .WithSpecialty(ServiceSpecialty.Paediatrics)
                .WithPerformerOrganisation(ReferrerOrganisation.Willow)
                .Build();

        private static CarePlan.ActivityComponent ReferralBookableNoSpecialty() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.bookable)
                .WithReferredDateDaysAgo(16)
                .WithSpecialty(ServiceSpecialty.None)
                .WithPerformerOrganisation(ReferrerOrganisation.Willow)
                .Build();

        private static CarePlan.ActivityComponent ReferralBookableWasCancelled() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.bookableWasCancelled)
                .WithReferredDateDaysAgo(9)
                .WithSpecialty(ServiceSpecialty.Neurology)
                .WithPerformerOrganisation(ReferrerOrganisation.Birch)
                .Build();

        private static CarePlan.ActivityComponent ReferralInReviewUrgent() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.bookableWasCancelled)
                .WithReferredDateDaysAgo(10)
                .WithSpecialty(ServiceSpecialty.None)
                .WithPerformerOrganisation(ReferrerOrganisation.Birch)
                .Build();

        private static CarePlan.ActivityComponent ReferralBookableWasCancelledNoSpecialty() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.bookableWasCancelled)
                .WithReferredDateDaysAgo(9)
                .WithSpecialty(ServiceSpecialty.None)
                .WithPerformerOrganisation(ReferrerOrganisation.Birch)
                .Build();
    }
}