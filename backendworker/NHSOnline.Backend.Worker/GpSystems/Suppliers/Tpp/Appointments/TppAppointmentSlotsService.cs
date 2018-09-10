using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
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
        public async Task<AppointmentSlotsResult> GetSlots(UserSession userSession, AppointmentSlotsDateRange dateRange)
        {
            try
            {
                _logger.LogEnter(nameof(GetSlots));
            
                var tppUserSession = (TppUserSession)userSession;
                var request = new ListSlots
                {
                    StartDate = dateRange.FromDate.DateTime,
                    NumberOfDays = (dateRange.ToDate - dateRange.FromDate).Days,
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
            finally
            {
                _logger.LogExit(nameof(GetSlots));
            }
        }
    }
}
