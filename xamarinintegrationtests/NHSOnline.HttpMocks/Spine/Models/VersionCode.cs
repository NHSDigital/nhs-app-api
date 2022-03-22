using System.Xml.Serialization;

namespace NHSOnline.HttpMocks.Spine.Models
{
    public class VersionCode
    {
        [XmlAttribute(AttributeName="code")]
		public string? Code { get; set; }
	}
}
