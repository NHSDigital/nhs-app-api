using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.GpSystems.Prescriptions.Models
{
    public class PrescriptionRequestPostPartialSuccessResponse
    {
        public IEnumerable<Order> SuccessfulOrders { get; set; } = Enumerable.Empty<Order>();

        public IEnumerable<Order> UnsuccessfulOrders { get; set; } = Enumerable.Empty<Order>();
    }
}
