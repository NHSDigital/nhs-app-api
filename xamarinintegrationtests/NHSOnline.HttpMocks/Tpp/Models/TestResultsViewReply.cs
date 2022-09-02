using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Tpp.Models
{
    public class TestResultsViewReply
    {
        [XmlElement("Item")]
        public Collection<TestResultsViewReplyItem>? Items { get; set; }
    }

    public class TestResultsViewReplyItem
    {
        [XmlAttribute("id")] public string? Id { get; set; }

        [XmlAttribute("description")] public string? Description { get; set; }

        [XmlAttribute("date")] public string? Date { get; set; }

        [XmlText] public string? Value { get; set; }
    }
}