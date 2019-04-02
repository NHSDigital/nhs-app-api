using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Appointments
{
    public interface ICancellationReasonService
    {
        IEnumerable<CancellationReason> GetDefaultCancellationReasons();

        bool TryGetCancellationReason(string id, out CancellationReason cancellationReason);
    }
}