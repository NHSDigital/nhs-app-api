using System;
using System.Xml.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class Logoff : AbstractTppRequestModel
    {
        [XmlIgnore]
        public override string RequestType => "Logoff";
    }
}