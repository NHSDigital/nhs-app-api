using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    internal class TppAppointmentSlotsService : IAppointmentSlotsService
    {
        private readonly ITppClient _tppClient;
        private readonly ILogger<TppAppointmentSlotsService> _logger;
        private readonly ITppClientRequest<(ListSlots listSlots, string suid), ListSlotsReply> _listSlots;
        private readonly IAppointmentSlotsMapper _appointmentSlotsMapper;
        public TppAppointmentSlotsService(
            ITppClient tppClient,
            ILogger<TppAppointmentSlotsService> logger,
            ITppClientRequest<(ListSlots listSlots, string suid), ListSlotsReply> listSlots,
            IAppointmentSlotsMapper appointmentSlotsMapper)
        {
            _tppClient = tppClient;
            _logger = logger;
            _listSlots = listSlots;
            _appointmentSlotsMapper = appointmentSlotsMapper;
        }
        public async Task<AppointmentSlotsResult> GetSlots(
            GpLinkedAccountModel gpLinkedAccountModel, AppointmentSlotsDateRange dateRange)
        {
            try
            {
                _logger.LogEnter();
            
                var tppUserSession = (TppUserSession) gpLinkedAccountModel.GpUserSession;
                var listSlotsRequest = new ListSlots(tppUserSession, dateRange);
                var listSlotsTask = _listSlots.Post((listSlotsRequest, tppUserSession.Suid));
                await listSlotsTask;

                var messagesRequest = new RequestSystmOnlineMessages(tppUserSession);
                var messagesTask = _tppClient.RequestSystmOnlineMessages(messagesRequest, tppUserSession.Suid);
                
                try
                {
                    await messagesTask;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception has been thrown calling TPP.");
                }

                var result = new TppAppointmentSlotsResultBuilder(_logger, _appointmentSlotsMapper)
                    .Build(listSlotsTask, messagesTask);

                return result.ValueOrFailure();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "HttpRequestException has been thrown.");
                return new AppointmentSlotsResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
