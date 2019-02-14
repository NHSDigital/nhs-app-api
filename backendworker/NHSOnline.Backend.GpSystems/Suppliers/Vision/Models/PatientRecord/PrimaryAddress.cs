using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    public class PrimaryAddress 
    {    
        [XmlElement(ElementName="houseName", Namespace="urn:vision")]
        public string HouseName { get; set; }
        
        [XmlElement(ElementName="houseNumber", Namespace="urn:vision")]
        public string HouseNumber { get; set; }
        
        [XmlElement(ElementName="street", Namespace="urn:vision")]
        public string Street { get; set; }
        
        [XmlElement(ElementName="town", Namespace="urn:vision")]
        public string Town { get; set; }
        
        [XmlElement(ElementName="county", Namespace="urn:vision")]
        public string County { get; set; }
        
        [XmlElement(ElementName="postcode", Namespace="urn:vision")]
        public string Postcode { get; set; }

    }
}