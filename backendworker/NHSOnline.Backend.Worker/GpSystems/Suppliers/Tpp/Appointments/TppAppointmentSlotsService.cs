using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppAppointmentSlotsService : IAppointmentSlotsService
    {
        private readonly ITppClient _tppClient;
        private readonly ILogger<TppAppointmentSlotsService> _logger;
        private readonly IAppointmentSlotResultBuilder _slotResultBuilder;
        public TppAppointmentSlotsService(
            ITppClient tppClient, 
            ILogger<TppAppointmentSlotsService> logger,
            IAppointmentSlotResultBuilder appointmentSlotResultBuilder)
        {
            _tppClient = tppClient;
            _logger = logger;
            _slotResultBuilder = appointmentSlotResultBuilder;
        }
        public async Task<AppointmentSlotsResult> Get(UserSession userSession, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            var tppUserSession = (TppUserSession)userSession;

            try
            {
                var request = new ListSlots
                {
                    StartDate = fromDate.DateTime,
                    NumberOfDays = (toDate - fromDate).Days,
                    UnitId = tppUserSession.UnitId,
                    PatientId = tppUserSession.PatientId,
                    OnlineUserId = tppUserSession.OnlineUserId
                };

                var listSlotsTask = _tppClient.ListSlotsPost(request, tppUserSession.Suid);
                await Task.WhenAll(listSlotsTask);

                var result = _slotResultBuilder.Build(listSlotsTask);

                return result.ValueOrFailure();

            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "HttpRequestException has been thrown.");
                return new AppointmentSlotsResult.InternalServerError();
            }
        }
    }
}