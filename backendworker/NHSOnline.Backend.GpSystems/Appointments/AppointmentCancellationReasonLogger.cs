using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Appointments
{
    public interface IAppointmentCancellationReasonLogger
    {
        void CaptureCancellationReasons(GpUserSession gpUserSession, AppointmentsResult result);
    }

    public class AppointmentCancellationReasonLogger : IAppointmentCancellationReasonLogger
    {
        private static readonly ConcurrentDictionary<string, CancellationReasonsValue> CapturedCancellationReasons =
            new ConcurrentDictionary<string, CancellationReasonsValue>();

        private class CancellationReasonsInformation
        {
            public IReadOnlyList<string> CancellationReasons { get; }

            public string Supplier { get; }

            public string OdsCode { get; }

            public CancellationReasonsInformation(string[] cancellationReasons, string supplier, string odsCode)
            {
                CancellationReasons = cancellationReasons;
                Supplier = supplier;
                OdsCode = odsCode;
            }
        }

        private class CancellationReasonsValue
        {
            public DateTime CurrentDate { get; }

            public int ReasonCount { get; }

            public CancellationReasonsValue(DateTime currentDate, int reasonCount)
            {
                CurrentDate = currentDate;
                ReasonCount = reasonCount;
            }
        }

        private readonly ILogger<AppointmentCancellationReasonLogger> _logger;

        public AppointmentCancellationReasonLogger(
            ILogger<AppointmentCancellationReasonLogger> logger)
        {
            _logger = logger;
        }

        public void CaptureCancellationReasons(GpUserSession gpUserSession, AppointmentsResult result)
        {
            if (!(result is AppointmentsResult.Success successfulResult)
                || successfulResult.Response.CancellationReasons?.Any() != true)
            {
                return;
            }

            var cancellationReasons = successfulResult.Response.CancellationReasons.Select(x => x.DisplayName)
                .Distinct().ToArray();
            var cancellationReasonsInformation = new CancellationReasonsInformation(cancellationReasons,
                gpUserSession.Supplier.ToString(), gpUserSession.OdsCode);

            if (!ShouldBeLogged(cancellationReasonsInformation))
            {
                return;
            }

            _logger.LogInformation("cancellation_reason_data=" + cancellationReasonsInformation.SerializeJson());
        }

        private static bool ShouldBeLogged(CancellationReasonsInformation cancellationReasonsInformation)
        {
            var cancellationReasonsValue =
                new CancellationReasonsValue(DateTime.Today, cancellationReasonsInformation.CancellationReasons.Count);

            CapturedCancellationReasons.TryGetValue(cancellationReasonsInformation.OdsCode, out var existingValue);

            if (existingValue == null ||
                existingValue.CurrentDate.Date != DateTime.Today.Date ||
                existingValue.ReasonCount < cancellationReasonsInformation.CancellationReasons.Count)
            {
                CapturedCancellationReasons.AddOrUpdate(cancellationReasonsInformation.OdsCode,
                    cancellationReasonsValue,
                    (key, oldValue) => cancellationReasonsValue);
                return true;
            }

            return false;
        }
    }
}