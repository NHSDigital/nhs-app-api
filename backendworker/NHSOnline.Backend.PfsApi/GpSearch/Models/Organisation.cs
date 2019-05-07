using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.GpSearch.Models
{
    public class Organisation
    {
        public string OrganisationName { get; set; }
        
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string Postcode { get; set; }

        public string NACSCode { get; set; }

        public Geocode Geocode { get; set; }

        public string OpeningTimes { get; set; }

        public string Contacts { get; set; }

        public IEnumerable<OpeningTime> GetOpeningTimesArray()
        {
            var listOfOpeningTimes = !string.IsNullOrEmpty(OpeningTimes) ?
                JsonConvert.DeserializeObject<IEnumerable<OpeningTime>>(OpeningTimes) : Enumerable.Empty<OpeningTime>();

            return listOfOpeningTimes ?? Enumerable.Empty<OpeningTime>();
        }

        public IEnumerable<ContactInformation> GetContactsArray()
        {
            var listOfContacts = !string.IsNullOrEmpty(Contacts)?
                JsonConvert.DeserializeObject<IEnumerable<ContactInformation>>(Contacts) : Enumerable.Empty<ContactInformation>();

            return listOfContacts ?? Enumerable.Empty<ContactInformation>();
        }
    }
}