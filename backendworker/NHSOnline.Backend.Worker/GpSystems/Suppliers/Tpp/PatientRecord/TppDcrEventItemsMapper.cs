using System;
using System.Collections.Generic;
using System.Globalization;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface ITppDcrEventItemsMapper
    {
        List<string> Map(List<RequestPatientRecordItem> eventItems);
    }
    
    public class TppDcrEventItemsMapper : ITppDcrEventItemsMapper
    {
        public List<string> Map(List<RequestPatientRecordItem> eventItems)
        {
            var recordItems = new List<string>();
            if (eventItems == null)
            {
                return recordItems;
            }

            foreach (var recordItem in eventItems)
            {
                if (recordItem != null)
                {
                    recordItems.Add(FormatEventItem(recordItem));
                }
            }

            return recordItems;
        }
        
        private static string FormatEventItem(RequestPatientRecordItem item)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} - {1}", 
                item.Type, 
                Decode(item.Details));
        }

        private static string Decode(string itemDetail)
        {
            if (string.IsNullOrWhiteSpace(itemDetail))
            {
                return string.Empty;
            }

            return itemDetail.Replace("\t", string.Empty, StringComparison.Ordinal)
                .Trim(new [] {'\n'})
                .Replace("\n", "; ", StringComparison.Ordinal);
        }
    }
}