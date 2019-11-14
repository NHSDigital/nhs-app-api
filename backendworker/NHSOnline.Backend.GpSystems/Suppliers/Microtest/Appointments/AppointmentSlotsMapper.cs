using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class AppointmentSlotsMapper : IAppointmentSlotsMapper
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly IMicrotestEnumMapper _enumMapper;
        private readonly ILogger<AppointmentSlotsMapper> _logger;

        public AppointmentSlotsMapper(IDateTimeOffsetProvider dateTimeOffsetProvider, IMicrotestEnumMapper enumMapper,
            ILogger<AppointmentSlotsMapper> logger)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _enumMapper = enumMapper;
            _logger = logger;
        }

        public IList<Slot> Map(
            IEnumerable<Suppliers.Microtest.Models.Appointments.Slot> sourceSlots)
        {
            _logger.LogEnter();

            var slots = new List<Slot>();

            if (sourceSlots == null || !sourceSlots.Any())
            {
                return slots;
            }

            foreach (var sourceSlot in sourceSlots)
            {
                if (TryMapSlot(sourceSlot, out var resultSlot))
                {
                    slots.Add(resultSlot);
                }
            }

            _logger.LogExit();
            return slots;
        }

        private bool TryMapSlot(Models.Appointments.Slot sourceSlot, out Slot resultSlot)
        {
            var startTime = ParseSlotTime(sourceSlot.StartTime, "Start");

            if (startTime == null)
            {
                resultSlot = null;
                return false;
            }

            resultSlot = new Slot();

            resultSlot.Id = sourceSlot.Id;
            resultSlot.StartTime = startTime.Value;
            resultSlot.EndTime = ParseSlotTime(sourceSlot.EndTime, "End");
            resultSlot.Clinicians = sourceSlot.Clinicians;
            resultSlot.Location = string.IsNullOrWhiteSpace(sourceSlot.Location)
                ? string.Empty
                : sourceSlot.Location;
            resultSlot.Type = string.IsNullOrWhiteSpace(sourceSlot.Type)
                ? string.Empty
                : sourceSlot.Type;
            resultSlot.SessionName = string.Empty;
            resultSlot.Channel = _enumMapper.MapChannel(sourceSlot.Channel, Channel.Unknown);

            return true;
        }

        private DateTimeOffset? ParseSlotTime(string time, string usage)
        {
            var success = _dateTimeOffsetProvider.TryCreateDateTimeOffset(time, out var parsedTime);
            if (!success)
            {
                _logger.LogError($"Unable to parse Microtest Appointment Slot {usage} Time of '{time}'");
            }

            return parsedTime;
        }
    }
}