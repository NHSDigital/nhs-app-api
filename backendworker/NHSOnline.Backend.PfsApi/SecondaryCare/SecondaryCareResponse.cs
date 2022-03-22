using System;
using System.Net;
using NHSOnline.Backend.PfsApi.SecondaryCare.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareResponse<TBody> : ApiResponse
    {
        public SecondaryCareResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();

        public SummaryResponse Body { get; private set; }

        public SecondaryCareResponse<TBody> Parse()
        {
            Body = GetSampleSummary();

            return this;
        }

        protected override bool FormatResponseIfUnsuccessful => false;

        private SummaryResponse GetSampleSummary()
        {
            return new SummaryResponse
            {
                Referrals = new[]
                {
                    new Referral
                    {
                        ReferralId = "28fa7a42-b6b4-4063-8052-171cd7c6bf34",
                        Provider = ReferralProvider.Ers.ToString(),
                        ReferredDateTime = new DateTime(2022, 1, 6, 8, 0, 0, DateTimeKind.Utc),
                        ReferrerOrganisation = "eRS",
                        ReviewDueDate = new DateTime(2022, 4, 6, 13, 10, 0, DateTimeKind.Utc),
                        ServiceSpeciality = "Cardiology",
                        Status = ReferralStatus.InReview.ToString(),
                    },
                    new Referral
                    {
                        ReferralId = "67cf3dc6-dc98-4154-b8f4-966cbb55ff7f",
                        Provider = ReferralProvider.Pkb.ToString(),
                        ReferredDateTime = new DateTime(2021, 2, 3, 9, 30, 0, DateTimeKind.Utc),
                        ReferrerOrganisation = "Patient Knows Best",
                        ReviewDueDate = new DateTime(2022, 2, 3, 17, 15, 0, DateTimeKind.Utc),
                        ServiceSpeciality = "Oncology",
                        Status = ReferralStatus.Bookable.ToString(),
                    },
                    new Referral
                    {
                        ReferralId = "467a3d7a-9a70-4595-addf-472507d0b95b",
                        Provider = ReferralProvider.Drdoctor.ToString(),
                        ReferredDateTime = new DateTime(2022, 3, 3, 10, 30, 0, DateTimeKind.Utc),
                        ReferrerOrganisation = "Dr. Doctor",
                        ReviewDueDate = new DateTime(2022, 9, 1, 11, 0, 0, DateTimeKind.Utc),
                        ServiceSpeciality = "Neurology",
                        Status = ReferralStatus.BookableWasCancelled.ToString(),
                    }
                },
                UpcomingAppointments = new[]
                {
                    new UpcomingAppointment
                    {
                        AppointmentId = "7d1f4365-c3b6-4669-af25-f3da03d2ec7b",
                        Provider = ReferralProvider.Ers.ToString(),
                        AppointmentDateTime = new DateTime(2022, 7, 6, 8, 0, 0, DateTimeKind.Utc),
                        ServiceSpeciality = "Cardiology",
                        LocationDescription = "City Hospital, Floor 2",
                        DeepLinkUrl = "https://www.google.co.uk",
                    },
                    new UpcomingAppointment
                    {
                        AppointmentId = "431eef4d-6554-43ae-9228-03b9e197825b",
                        Provider = ReferralProvider.Pkb.ToString(),
                        AppointmentDateTime = new DateTime(2022, 8, 3, 9, 30, 0, DateTimeKind.Utc),
                        ServiceSpeciality = "Oncology",
                        LocationDescription = "City Hospital, Floor 3",
                        DeepLinkUrl = "https://www.google.co.uk",
                    },
                    new UpcomingAppointment
                    {
                        AppointmentId = "2da5d68d-5b78-4eb0-bf73-9b392935d15a",
                        Provider = ReferralProvider.Drdoctor.ToString(),
                        AppointmentDateTime = new DateTime(2022, 9, 3, 10, 30, 0, DateTimeKind.Utc),
                        ServiceSpeciality = "Neurology",
                        LocationDescription = "City Hospital, Floor 4",
                        DeepLinkUrl = "https://www.google.co.uk",
                    }
                },
            };
        }
    }
}