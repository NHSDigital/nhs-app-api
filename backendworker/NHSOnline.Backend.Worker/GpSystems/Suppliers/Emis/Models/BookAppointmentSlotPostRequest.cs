using System;
using System.Globalization;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models
{
    public class BookAppointmentSlotPostRequest
    {
       public BookAppointmentSlotPostRequest (EmisUserSession emisUserSession, AppointmentBookRequest request)
       {
           UserPatientLinkToken = emisUserSession.UserPatientLinkToken;
           BookingReason = request.BookingReason;
           SlotId = Convert.ToInt64(request.SlotId, CultureInfo.InvariantCulture);
           
           if (!String.IsNullOrWhiteSpace(request.TelephoneNumber))
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
