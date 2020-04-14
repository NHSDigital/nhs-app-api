
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentsLoggingVisitor : IAppointmentsResultVisitor<Task>
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly P9UserSession _userSession;

        public AppointmentsLoggingVisitor(
            ILogger<AppointmentsController> logger,
            P9UserSession userSession
        )
        {
            _logger = logger;
            _userSession = userSession;
        }

        public Task Visit(AppointmentsResult.Success result)
        {
            try
            {
                LogAppointmentsInformation(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to log appointment details. " +
                                    "Catching exception to prevent inability to view appointments");
            }

            return Task.CompletedTask;
        }

        private void LogAppointmentsInformation(AppointmentsResult.Success result)
        {
            var response = result?.Response;

            var upcomingAppointmentsCount = response?.UpcomingAppointments?.Count() ?? 0;
            var pastAppointmentsCount = response?.PastAppointments?.Count() ?? 0;

            var gpUserSession = _userSession.GpUserSession;

            var kvp = new Dictionary<string, string>
            {
                { "Supplier", gpUserSession.Supplier.ToString() },
                { "OdsCode", gpUserSession.OdsCode },
                { "Count", (upcomingAppointmentsCount+pastAppointmentsCount).ToString(CultureInfo.InvariantCulture) },
                { "UpcomingCount", upcomingAppointmentsCount.ToString(CultureInfo.InvariantCulture) },
                { "HistoricalCount", pastAppointmentsCount.ToString(CultureInfo.InvariantCulture)}
            };

            _logger.LogInformationKeyValuePairs("Appointment Count", kvp);
        }

        #region No-ops for unsuccessful results
        public Task Visit(AppointmentsResult.BadRequest result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(AppointmentsResult.BadGateway result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(AppointmentsResult.InternalServerError result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(AppointmentsResult.Forbidden result)
        {
            return Task.CompletedTask;
        }
        #endregion
    }
}