using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class TppDcrEventsMapper
    {
        public TppDcrEvents Map(RequestPatientRecordReply requestPatientRecordReply)
        {
            if (requestPatientRecordReply == null)
            {
                throw new ArgumentNullException(nameof(requestPatientRecordReply));
            }

            return new TppDcrEvents
            {    
                Data = requestPatientRecordReply.Events != null ?
                        (from dcrEvent in requestPatientRecordReply.Events
                            select new TppDcrEvent
                            {
                                Date = DateTimeOffset.Parse(dcrEvent.Date, CultureInfo.InvariantCulture),
                                LocationAndDoneBy = string.Format(CultureInfo.InvariantCulture, "{0} - {1}", dcrEvent.Location, dcrEvent.DoneBy),
                                EventItems = dcrEvent.Items != null
                                    ? (from item in dcrEvent.Items
                                        select string.Format(CultureInfo.InvariantCulture, "{0} - {1}", 
                                            item.Type, 
                                            !string.IsNullOrEmpty(item.Details) ? 
                                                item.Details
                                                    .Replace("\t", string.Empty, StringComparison.Ordinal)
                                                    .Trim(new [] {'\n'})
                                                    .Replace("\n", "; ", StringComparison.Ordinal) : string.Empty)).ToList()
                                    : new List<string>(),
                            }
                        ).ToList() : new List<TppDcrEvent>()
            };
        }     
    }
}