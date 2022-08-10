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
                {"2014105131", new List<CarePlan.ActivityComponent>()},
                {
                    "2014105132", new List<CarePlan.ActivityComponent>()
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
                .Build();

        private static CarePlan.ActivityComponent ReferralInReviewPkb() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.inReview)
                .WithReferredDateDaysAgo(3)
                .WithSpecialty(ServiceSpecialty.General)
                .WithProvider(ServiceProvider.PKB)
                .Build();

        private static CarePlan.ActivityComponent ReferralInReviewDue() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.inReview)
                .WithReferredDateDaysAgo(8)
                .WithDueDateInDays(5)
                .WithSpecialty(ServiceSpecialty.Cardiology)
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
                .Build();

        private static CarePlan.ActivityComponent ReferralInReviewOverdueNoSpecialty() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.inReview)
                .WithReferredDateDaysAgo(5)
                .WithMissedDueDateDaysAgo(2)
                .WithSpecialty(ServiceSpecialty.None)
                .Build();

        private static CarePlan.ActivityComponent ReferralBookable() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.bookable)
                .WithReferredDateDaysAgo(16)
                .WithSpecialty(ServiceSpecialty.Paediatrics)
                .Build();

        private static CarePlan.ActivityComponent ReferralBookableNoSpecialty() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.bookable)
                .WithReferredDateDaysAgo(16)
                .WithSpecialty(ServiceSpecialty.None)
                .Build();

        private static CarePlan.ActivityComponent ReferralBookableWasCancelled() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.bookableWasCancelled)
                .WithReferredDateDaysAgo(9)
                .WithSpecialty(ServiceSpecialty.Neurology)
                .Build();

        private static CarePlan.ActivityComponent ReferralInReviewUrgent() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.bookableWasCancelled)
                .WithReferredDateDaysAgo(10)
                .WithSpecialty(ServiceSpecialty.None)
                .Build();

        private static CarePlan.ActivityComponent ReferralBookableWasCancelledNoSpecialty() =>
            new ReferralBuilder()
                .WithReferralStatus(ReferralStatus.bookableWasCancelled)
                .WithReferredDateDaysAgo(9)
                .WithSpecialty(ServiceSpecialty.None)
                .Build();
    }
}