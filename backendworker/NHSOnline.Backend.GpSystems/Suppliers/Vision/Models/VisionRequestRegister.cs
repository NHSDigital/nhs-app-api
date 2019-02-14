using System.Xml.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public class VisionRequestRegister<T>
    {

        [XmlElement(ElementName = "register", Namespace = "urn:vision")]
        public ServiceContentRegister ServiceContentRegister { get; set; }
        
        public VisionRequestRegister(string accountId, string linkageKey, string surname, string dob, string providerId)
        {
            ServiceContentRegister = new ServiceContentRegister
            {
                RosuAccountId = accountId,
                RosuAccountLinkageKey = linkageKey,
                Surname = surname,
                Dob = dob,
                
                OpsReference = new OpsReference
                {
                    AccountId = providerId,
                    Provider = providerId
                }
            };
        }

        public VisionRequestRegister()
        {
            
        }
    }
}