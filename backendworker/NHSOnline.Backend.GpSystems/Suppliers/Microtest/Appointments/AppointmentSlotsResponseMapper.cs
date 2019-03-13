using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public interface IAppointmentSlotsResponseMapper
    {
        AppointmentSlotsResponse Map(AppointmentSlotsGetResponse slotsResponse);
    }

    public class AppointmentSlotsResponseMapper : IAppointmentSlotsResponseMapper
    {
        private readonly IMicrotestEnumMapper _enumMapper;
        private readonly ILogger<AppointmentSlotsResponseMapper> _logger;

        public AppointmentSlotsResponseMapper(IMicrotestEnumMapper enumMapper, ILogger<AppointmentSlotsResponseMapper> logger)
        {
            _enumMapper = enumMapper;
            _logger = logger;
        }

        public AppointmentSlotsResponse Map(AppointmentSlotsGetResponse slotsResponse)
        {
            _logger.LogEnter();

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
                    Channel = _enumMapper.MapChannel(sourceSlot.Channel, Channel.Unknown)
                };

                slots.Add(resultSlot);
            }

            var response = new AppointmentSlotsResponse
            {
                Slots = slots,
                BookingReasonNecessity = Necessity.Mandatory,
                BookingGuidance = string.Empty
            };

            _logger.LogExit();
            return response;
        }
    }
}