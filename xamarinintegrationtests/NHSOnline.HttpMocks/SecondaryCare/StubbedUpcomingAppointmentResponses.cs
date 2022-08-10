using System.Collections.Generic;
using NHSOnline.HttpMocks.SecondaryCare.Builders;
using Hl7.Fhir.Model;

namespace NHSOnline.HttpMocks.SecondaryCare
{
    public static class StubbedUpcomingAppointmentResponses
    {
        public static Dictionary<string, IList<CarePlan.ActivityComponent>> UpcomingAppointmentMapping { get; } =
            new()
            {
                {"2014105131", new List<CarePlan.ActivityComponent>()},
                {
                    "2014105132", new List<CarePlan.ActivityComponent>()
                    {
                        UpcomingAppointmentWithId(),
                        UpcomingAppointment(),
                        UpcomingAppointmentWithSpecialty(),
                        UpcomingAppointmentWithDateInDays(5),
                        UpcomingAppointmentWithDateInDays(26),
                        UpcomingAppointmentCancelledWithDateInDays(30),
                        UpcomingAppointmentCancelledWithDateInDays(4),
                        UpcomingAppointmentCancelledWithDateInDays(-1),
                    }
                },
            };

        private static CarePlan.ActivityComponent UpcomingAppointment() =>
            new UpcomingAppointmentBuilder()
                .WithProvider(ServiceProvider.eRS)
                .WithAppointmentStatus(Appointment.AppointmentStatus.Booked)
                .WithLocationDescription("The Royal Victoria Hospital, Belfast, BT1")
                .Build();

        private static CarePlan.ActivityComponent UpcomingAppointmentWithId() =>
            new UpcomingAppointmentBuilder()
                .WithProvider(ServiceProvider.PKB)
                .WithAppointmentStatus(Appointment.AppointmentStatus.Booked)
                .WithLocationDescription("The Royal Victoria Hospital, Belfast, BT1")
                .WithSpecialty(ServiceSpecialty.Cardiology)
                .Build();

        private static CarePlan.ActivityComponent UpcomingAppointmentWithDateInDays(int days) =>
            new UpcomingAppointmentBuilder()
                .WithProvider(ServiceProvider.DrDoctor)
                .WithAppointmentStatus(Appointment.AppointmentStatus.Booked)
                .WithLocationDescription("The Royal Victoria Hospital, Belfast, BT1")
                .WithAppointmentDateInDays(days)
                .Build();

        private static CarePlan.ActivityComponent UpcomingAppointmentWithSpecialty() =>
            new UpcomingAppointmentBuilder()
                .WithProvider(ServiceProvider.DrDoctor)
                .WithAppointmentStatus(Appointment.AppointmentStatus.Booked)
                .WithSpecialty(ServiceSpecialty.Haematology)
                .WithLocationDescription("The Royal Victoria Hospital, Belfast, BT1")
                .Build();

        private static CarePlan.ActivityComponent UpcomingAppointmentWithProviderUbrn(
            ServiceProvider serviceProvider, string providerUbrn, int? days = 3) =>
            new UpcomingAppointmentBuilder()
                .WithProvider(serviceProvider)
                .WithProviderUbrn(providerUbrn)
                .WithAppointmentStatus(Appointment.AppointmentStatus.Booked)
                .WithAppointmentDateInDays(days)
                .WithSpecialty(ServiceSpecialty.Haematology)
                .WithLocationDescription("The Royal Victoria Hospital, Belfast, BT1")
                .Build();

        private static CarePlan.ActivityComponent UpcomingAppointmentCancelledWithDateInDays(int days) =>
            new UpcomingAppointmentBuilder()
                .WithProvider(ServiceProvider.DrDoctor)
                .WithAppointmentStatus(Appointment.AppointmentStatus.Cancelled)
                .WithLocationDescription("The Royal Victoria Hospital, Belfast, BT1")
                .WithAppointmentDateInDays(days)
                .Build();
    }
}