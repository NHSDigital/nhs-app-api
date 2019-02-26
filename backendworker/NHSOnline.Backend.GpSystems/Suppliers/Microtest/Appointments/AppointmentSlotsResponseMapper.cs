using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public interface IAppointmentSlotsResponseMapper
    {
        AppointmentSlotsResponse Map(AppointmentSlotsGetResponse slotsResponse);
    }

    public class AppointmentSlotsResponseMapper : IAppointmentSlotsResponseMapper
    {
        public AppointmentSlotsResponse Map(AppointmentSlotsGetResponse slotsResponse)
        {
            var slots = new List<GpSystems.Appointments.Models.Slot>();

            foreach (var sourceSlot in slotsResponse.Slots)
            {
                if (sourceSlot.StartTime == null)
                {
                    continue;
                }

                var resultSlot = new GpSystems.Appointments.Models.Slot
                {
                    Id = sourceSlot.Id,
                    StartTime = sourceSlot.StartTime.Value,
                    EndTime = sourceSlot.EndTime,
                    Clinicians = sourceSlot.Clinicians,
                    Location = sourceSlot.Location,
                    Type = sourceSlot.Type,
                    SessionName = string.Empty,
                };

                slots.Add(resultSlot);
            }

            // TODO
            // Will Microtest accept booking reason? We should set BookingReasonNecessity in the response here.

            var response = new AppointmentSlotsResponse
            {
                Slots = slots,
            };

            return response;
        }
    }
}