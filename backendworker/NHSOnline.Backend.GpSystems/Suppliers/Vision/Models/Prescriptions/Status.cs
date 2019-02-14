using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Prescriptions
{
    public class Status
    {
        // <summary>
        // See <see cref="PrescriptionRepeatStatusCode" for possible values.
        // Not using enum as status values can be negative and zero
        // is already an existing option so not a good value to default to
        // if we receive any unrecognized value. />
        // </summary>
        [XmlAttribute(AttributeName = "code")]
        public short Code { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}
