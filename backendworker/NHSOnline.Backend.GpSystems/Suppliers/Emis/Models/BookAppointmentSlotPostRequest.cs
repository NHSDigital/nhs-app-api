using System;
using System.Globalization;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class BookAppointmentSlotPostRequest
    {
       public BookAppointmentSlotPostRequest (string userPatientLinkToken, AppointmentBookRequest request)
       {
           UserPatientLinkToken = userPatientLinkToken;
           BookingReason = request.BookingReason;
           SlotId = Convert.ToInt64(request.SlotId, CultureInfo.InvariantCulture);
           
           if (!string.IsNullOrWhiteSpace(request.TelephoneNumber))
           {
               TelephoneNumber = request.TelephoneNumber;
               TelephoneContactType = "Other";
           }
       }

        public string UserPatientLinkToken { get;  }
        public long SlotId { get;  }
        public string BookingReason { get;  }
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public string TelephoneNumber { get;  }
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public string TelephoneContactType { get;  } 
    }
}
