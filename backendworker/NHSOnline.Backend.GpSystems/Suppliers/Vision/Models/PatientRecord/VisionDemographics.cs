using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord
{
    public class VisionDemographics
    {
        [XmlElement(ElementName="name", Namespace="urn:vision")]
        public Name Name { get; set; }
        
        [XmlElement(ElementName="maritalStatus", Namespace="urn:vision")]
        public MaritalStatus MaritalStatus { get; set; }
        
        [XmlElement(ElementName="dateOfBirth", Namespace="urn:vision")]
        public DateTime DateOfBirth { get; set; }
        
        [XmlElement(ElementName="gender", Namespace="urn:vision")]
        public Gender Gender { get; set; }
        
        [XmlElement(ElementName="primaryAddress", Namespace="urn:vision")]
        public PrimaryAddress PrimaryAddress { get; set; }
    }
}