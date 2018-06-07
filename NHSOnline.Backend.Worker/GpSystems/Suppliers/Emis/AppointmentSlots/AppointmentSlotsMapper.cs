using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Date;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.AppointmentSlots
{
    public class AppointmentSlotsMapper
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

        public AppointmentSlotsMapper(IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }
        public Slot[] Map(AppointmentsSlotsGetResponse slotsResponse, AppointmentSlotsMetadataGetResponse slotsMetadataResponse)
        {
            var slots = new List<Slot>();

            if (slotsResponse.Sessions == null || slotsMetadataResponse.Sessions == null)
            {
                return slots.ToArray();
            }
            
            if (!slotsResponse.Sessions.Any() || !slotsMetadataResponse.Sessions.Any())
            {
                return slots.ToArray();
            }
            
            foreach (var appointmentSlotSession in slotsResponse.Sessions)
            {
                foreach (var appointmentSlot in appointmentSlotSession.Slots)
                {
                    DateTimeOffset startTime;
                    
                    try
                    {
                        startTime = _dateTimeOffsetProvider.CreateDateTimeOffset(appointmentSlot.StartTime).ToUniversalTime();
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    DateTimeOffset? endTime;
                    try
                    {
                        endTime = _dateTimeOffsetProvider.CreateDateTimeOffset(appointmentSlot.EndTime).ToUniversalTime();
                    }
                    catch (Exception)
                    {
                        endTime = null;
                    }
                    
                    var sessionId = appointmentSlotSession.SessionId;
                    var slot = new Slot()
                    {
                        Id = appointmentSlot.SlotId.ToString(),
                        AppointmentSessionId = sessionId.ToString(),
                        StartTime = startTime,
                        EndTime = endTime,
                        ClinicianIds = ExtractCliniciansIds(sessionId, slotsMetadataResponse.Sessions),
                        LocationId = ExtractLocationsId(sessionId, slotsMetadataResponse.Sessions)
                    };

                    slots.Add(slot);
                }
            }

            return slots.ToArray();
        }

        private string[] ExtractCliniciansIds(int sessionId, IEnumerable<Models.Session> sessions)
        {
            var clinicianIds = new List<string>();
            foreach (var session in sessions)
            {
                if (sessionId == session.SessionId)
                {
                    clinicianIds.AddRange(session.ClinicianIds.Select(sessionClinicianId => sessionClinicianId.ToString()));
                }
            }

            return clinicianIds.ToArray();
        }
        
        private string ExtractLocationsId(int sessionId, IEnumerable<Models.Session> sessions)
        {
            foreach (var session in sessions)
            {
                if (sessionId == session.SessionId)
                {
                    return session.LocationId.ToString();
                }
            }

            return "";
        }
        
    }
}
