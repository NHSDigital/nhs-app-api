using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Resources;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Appointments
{
    public class CancellationReasonService : ICancellationReasonService
    {
        private static readonly List<CancellationReason> CancellationReasons = InitializeCancellationReasons();

        private static List<CancellationReason> InitializeCancellationReasons()
        {
            var resourceManager = new ResourceManager(typeof(CancellationReasons));

            var resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);
            var reasons = resourceSet.Cast<DictionaryEntry>()
                .OrderBy(r => r.Value.ToString())
                .Select(r => new CancellationReason
                {
                    Id = r.Key.ToString(),
                    DisplayName = r.Value.ToString()
                })
                .ToList();

            return reasons;
        }

        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Intentional; do not wish consumers to treat this as a property")]
        public IEnumerable<CancellationReason> GetDefaultCancellationReasons()
        {
            return CancellationReasons;
        }

        public bool TryGetCancellationReason(string id, out CancellationReason cancellationReason)
        {
            cancellationReason =
                CancellationReasons.SingleOrDefault(r => r.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            return cancellationReason != null;
        }
    }
}

