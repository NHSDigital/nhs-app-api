using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public class ViewPatientOverviewReply
    {
        [XmlArray("DrugSensitivities")]
        [XmlArrayItem("Item", Type = typeof(ViewPatientOverViewItem))]
        public Collection<ViewPatientOverViewItem>? DrugSensitivities { get; }

        [XmlArray("Drugs")]
        [XmlArrayItem("Item", Type = typeof(ViewPatientOverViewItem))]

        public Collection<ViewPatientOverViewItem>? Drugs { get; set; } = new Collection<ViewPatientOverViewItem>();

        [XmlArray("PastRepeats")]
        [XmlArrayItem("Item", Type = typeof(ViewPatientOverViewItem))]
        public Collection<ViewPatientOverViewItem>? PastRepeats { get; set; } =
            new Collection<ViewPatientOverViewItem>();

        [XmlArray("CurrentRepeats")]
        [XmlArrayItem("Item", Type = typeof(ViewPatientOverViewItem))]
        public Collection<ViewPatientOverViewItem>? CurrentRepeats { get; set; } =
            new Collection<ViewPatientOverViewItem>();

        [XmlArray("Allergies")]
        [XmlArrayItem("Item", Type = typeof(ViewPatientOverViewItem))]
        public Collection<ViewPatientOverViewItem>? Allergies { get; set; } = new Collection<ViewPatientOverViewItem>();
    }
}