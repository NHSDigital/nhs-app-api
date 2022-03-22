using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.SecondaryCare;
using NHSOnline.Backend.PfsApi.SecondaryCare;
using NHSOnline.Backend.PfsApi.SecondaryCare.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.SecondaryCare
{
    internal sealed class SecondaryCareControllerTestContext
    {
        internal TestMocks Mocks { get; }
        internal TestData Data { get; }
        private ServiceCollection ServiceCollection { get; }
        private ServiceProvider ServiceProvider { get; set; }

        public SecondaryCareControllerTestContext()
        {
            Mocks = new TestMocks();
            Data = new TestData();

            ServiceCollection = new ServiceCollection();

            InitializeServiceProvider();
        }

        private void InitializeServiceProvider()
        {
            ServiceCollection.AddTransient<SecondaryCareController>();

            new PfsApi.SecondaryCare.ServiceConfigurationModule().ConfigureServices(ServiceCollection, null);

            Mocks.ConfigureServices(ServiceCollection);

            ServiceProvider = ServiceCollection.BuildServiceProvider();
        }

        internal SecondaryCareController CreateSystemUnderTest() => ServiceProvider.GetRequiredService<SecondaryCareController>();

        internal SecondaryCareControllerTestContext MockSecondaryCareClient()
        {
            ServiceProvider = ServiceCollection
                .AddSingleton(Mocks.SecondaryCareClient.Object)
                .BuildServiceProvider();

            return this;
        }

        internal void MockSecondaryCareClientGetSummaryReturnsUnsuccessfulResponse()
        {
            Mocks.SecondaryCareClient
                .Setup(c => c.GetSummary())
                .Returns(new SecondaryCareResponse<SummaryResponse>(HttpStatusCode.BadRequest));
        }

        internal sealed class TestData
        {
            public P9UserSession P9UserSession { get; } = new P9UserSession("csrfToken", "111 111 1111", new CitizenIdUserSession(), "im1ConnectionToken");

            public SummaryResponse HardcodedSummaryResponse { get; } = new SummaryResponse
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

        internal sealed class TestMocks
        {
            internal Mock<ILogger<SecondaryCareController>> Logger { get; }
                = new Mock<ILogger<SecondaryCareController>>();
            internal Mock<IAuditor> Auditor { get; }
                = new Mock<IAuditor>();

            internal Mock<ISecondaryCareClient> SecondaryCareClient { get; }
                = new Mock<ISecondaryCareClient>();

            public void ConfigureServices(IServiceCollection serviceCollection)
            {
                serviceCollection
                    .AddSingleton(Auditor.Object)
                    .AddSingleton(Logger.Object)
                    .AddMockLoggers();
            }
        }
    }
}
