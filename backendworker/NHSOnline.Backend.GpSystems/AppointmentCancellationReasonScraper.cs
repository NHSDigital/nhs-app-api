using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems
{
    public interface IAppointmentCancellationReasonScraper
    {
        void CaptureCancellationReasons(GpUserSession gpUserSession, AppointmentsResult result);
    }
    
    public class AppointmentCancellationReasonScraper : IAppointmentCancellationReasonScraper
    {
        private static readonly ConcurrentDictionary<string, CancellationReasonsValue> CapturedCancellationReasons = 
            new ConcurrentDictionary<string, CancellationReasonsValue>();
        
        private class CancellationReasonsInformation
        {
            public string[] CancellationReasons { get; }
            
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

        private readonly ILogger<AppointmentCancellationReasonScraper> _logger;

        public AppointmentCancellationReasonScraper(
            ILogger<AppointmentCancellationReasonScraper> logger)
        {
            _logger = logger;
        }

        public void CaptureCancellationReasons(GpUserSession gpUserSession, AppointmentsResult result)
        {
            if (!(result is AppointmentsResult.SuccessfullyRetrieved successfulResult)
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
                new CancellationReasonsValue(DateTime.Today, cancellationReasonsInformation.CancellationReasons.Length);

            CapturedCancellationReasons.TryGetValue(cancellationReasonsInformation.OdsCode, out var existingValue);

            if (existingValue == null ||
                existingValue.CurrentDate.Date != DateTime.Today.Date ||
                existingValue.ReasonCount < cancellationReasonsInformation.CancellationReasons.Length)
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