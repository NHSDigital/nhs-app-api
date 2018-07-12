using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord
{
    public class TestResultsViewReply
    {       
        [XmlElement("Item")]
        public List<TestResultsViewReplyItem> Items { get; set; }          
    }
}