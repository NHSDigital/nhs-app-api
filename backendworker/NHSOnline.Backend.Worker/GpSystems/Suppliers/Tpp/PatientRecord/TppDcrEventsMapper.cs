using System;
using System.Collections.Generic;
using System.Globalization;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface ITppDcrEventsMapper
    {
        TppDcrEvents Map(RequestPatientRecordReply requestPatientRecordReply);
    }
    
    public class TppDcrEventsMapper : ITppDcrEventsMapper
    {
        private readonly ITppDcrEventItemsMapper _eventItemsMapper;

        public TppDcrEventsMapper(ITppDcrEventItemsMapper eventItemsMapper)
        {
            _eventItemsMapper = eventItemsMapper;
        }
        
        public TppDcrEvents Map(RequestPatientRecordReply requestPatientRecordReply)
        {
            if (requestPatientRecordReply == null)
            {
                throw new ArgumentNullException(nameof(requestPatientRecordReply));
            }

            return new TppDcrEvents
            {    
                Data = MapEvents(requestPatientRecordReply.Events)
            };
        }

        private List<TppDcrEvent> MapEvents(List<Event> events)
        {
            var mappedEvents = new List<TppDcrEvent>();
            if (events == null)
            {
                return mappedEvents;
            }

            foreach (var dcrEvent in events)
            {
                mappedEvents.Add(MapEvent(dcrEvent));
            }

            return mappedEvents;
        }

        private TppDcrEvent MapEvent(Event inputEvent)
        {
            return new TppDcrEvent
            {
                Date = inputEvent.Date.SafeParseToNullableDateTimeOffset(),
                LocationAndDoneBy = string.Format(CultureInfo.InvariantCulture, "{0} - {1}",
                    inputEvent.Location, inputEvent.DoneBy),
                EventItems = _eventItemsMapper.Map(inputEvent.Items)
            };
        }
    }
}