using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;
using NHSOnline.Backend.Support.Logging;
using Slot = NHSOnline.Backend.GpSystems.Appointments.Models.Slot;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public interface IAppointmentSlotsResponseMapper
    {
        AppointmentSlotsResponse Map(
            AppointmentSlotsGetResponse slotsResponse, 
            DemographicsGetResponse demographicsResponse
        );
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

        public AppointmentSlotsResponse Map(
            AppointmentSlotsGetResponse slotsResponse, 
            DemographicsGetResponse demographicsResponse)
        {
            _logger.LogEnter();

            var slots = MapSlots(slotsResponse);
            var telephoneNumbers = MapTelephoneNumbers(demographicsResponse);

            var response = new AppointmentSlotsResponse
            {
                Slots = slots,
                BookingReasonNecessity = Necessity.Mandatory,
                BookingGuidance = string.Empty,
                TelephoneNumbers = telephoneNumbers
            };

            _logger.LogExit();
            return response;
        }

        private IEnumerable<Slot> MapSlots(AppointmentSlotsGetResponse slotsResponse)
        {
            foreach (var sourceSlot in slotsResponse.Slots)
            {
                if (sourceSlot.StartTime == null)
                {
                    continue;
                }

                yield return new Slot
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
            }
        }

        private static IEnumerable<PatientTelephoneNumber> MapTelephoneNumbers(DemographicsGetResponse demographicsResponse)
        {
            if (demographicsResponse?.Demographics == null)
            {
                return Array.Empty<PatientTelephoneNumber>();
            }
            
             return MapTelephoneNumbers(new[]{
                demographicsResponse.Demographics.Telephone1,
                demographicsResponse.Demographics.Telephone2});
        }

        private static IEnumerable<PatientTelephoneNumber> MapTelephoneNumbers(IEnumerable<string> sourceNumbers)
        {
            foreach (var sourceNumber in sourceNumbers)
            {
                if (!string.IsNullOrEmpty(sourceNumber))
                {
                    yield return new PatientTelephoneNumber()
                    {
                        TelephoneNumber = sourceNumber
                    };
                }
            }
        }
    }
}