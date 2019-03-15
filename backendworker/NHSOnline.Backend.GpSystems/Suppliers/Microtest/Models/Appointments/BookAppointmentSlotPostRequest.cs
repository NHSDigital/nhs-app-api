using System;
using System.Globalization;
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
            SlotId = Convert.ToInt64(request.SlotId, CultureInfo.InvariantCulture);

            if (!string.IsNullOrWhiteSpace(request.TelephoneNumber))
            {
                TelephoneNumber = request.TelephoneNumber;
            }
        }

        [JsonProperty("slotId")]
        public long SlotId { get;  }
        [JsonProperty("bookingReason")]
        public string BookingReason { get;  }
        [JsonProperty("telephoneNumber", NullValueHandling=NullValueHandling.Ignore)]
        public string TelephoneNumber { get;  }
    }
}