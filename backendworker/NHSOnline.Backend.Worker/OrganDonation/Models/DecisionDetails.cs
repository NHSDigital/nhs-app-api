using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.OrganDonation.Models
{
    public class DecisionDetails
    {
        public bool All { get; set; }
        
        public IDictionary<string,ChoiceState> Choices { get; set; }
    }
}