using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    [XmlType("Events")]
    [SuppressMessage("Microsoft.Naming", "CA1716",
        Justification = "Deliberately matching the name specified by the GPSS")]
    public class Event
    {
        [XmlAttribute("date")] public string? Date { get; set; }

        [XmlAttribute("doneBy")] public string? DoneBy { get; set; }

        [XmlAttribute("location")] public string? Location { get; set; }

        [XmlElement("Item")]
        public Collection<RequestPatientRecordItem>? Items { get; set; }
    }
}