using System;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments
{
    [Serializable]
    public class BookAppointmentSlotPostRequest
    {
        public BookAppointmentSlotPostRequest(AppointmentBookRequest request)
        {
            BookingReason = request.BookingReason;
            SlotId = request.SlotId;

            if (!string.IsNullOrWhiteSpace(request.TelephoneNumber))
            {
                TelephoneNumber = request.TelephoneNumber;
            }
        }

        [JsonProperty("slotId")]
        public string SlotId { get;  }
        [JsonProperty("bookingReason")]
        public string BookingReason { get;  }
        [JsonProperty("telephoneNumber", NullValueHandling=NullValueHandling.Ignore)]
        public string TelephoneNumber { get;  }
    }
}