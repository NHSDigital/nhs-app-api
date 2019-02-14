using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
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
        public async Task<AppointmentSlotsResult> GetSlots(GpUserSession gpUserSession, AppointmentSlotsDateRange dateRange)
        {
            try
            {
                _logger.LogEnter();
            
                var tppUserSession = (TppUserSession) gpUserSession;
                var request = new ListSlots(tppUserSession, dateRange);
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
                _logger.LogExit();
            }
        }
    }
}
