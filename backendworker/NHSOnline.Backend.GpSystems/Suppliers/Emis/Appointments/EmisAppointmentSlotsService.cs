using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments
{
    public class EmisAppointmentSlotsService : IAppointmentSlotsService
    {
        private readonly IEmisClient _emisClient;
        private readonly ILogger<EmisAppointmentSlotsService> _logger;
        private readonly IAppointmentSlotsResponseMapper _appointmentSlotsResponseMapper;

        public EmisAppointmentSlotsService(
            IEmisClient emisClient,
            ILogger<EmisAppointmentSlotsService> logger,
            IAppointmentSlotsResponseMapper appointmentSlotsResponseMapper)
        {
            _emisClient = emisClient;
            _logger = logger;
            _appointmentSlotsResponseMapper = appointmentSlotsResponseMapper;
        }

        public async Task<AppointmentSlotsResult> GetSlots(
            GpLinkedAccountModel gpLinkedAccountModel, AppointmentSlotsDateRange dateRange)
        {
            try
            {
                _logger.LogEnter();

                var emisUserSession = (EmisUserSession) gpLinkedAccountModel.GpUserSession;
                var emisRequestParameters = gpLinkedAccountModel.BuildEmisRequestParameters(_logger); 
                var patientLinkToken = emisRequestParameters.UserPatientLinkToken;
                var metaParams = new SlotsMetadataGetQueryParameters(dateRange.FromDate, dateRange.ToDate, patientLinkToken);
                var slotsParams = new SlotsGetQueryParameters(dateRange.FromDate, dateRange.ToDate, patientLinkToken);

                _logger.LogInformation("Creating appointment slots requests");
                var metaTask = _emisClient.AppointmentSlotsMetadataGet(emisRequestParameters, metaParams);
                var slotTask = _emisClient.AppointmentSlotsGet(emisRequestParameters, slotsParams);

                await Task.WhenAll(metaTask, slotTask);
                _logger.LogInformation("Appointment slot requests completed");

                var practiceTask = _emisClient.PracticeSettingsGet(
                    emisRequestParameters, gpLinkedAccountModel.GpUserSession.OdsCode);
                
                var demographicsTask = _emisClient.DemographicsGet(emisRequestParameters);
                
                // Wait for practice and demographics tasks to complete, but unlike the other tasks suppress any errors such as timeout.
                try
                {
                    await Task.WhenAll(practiceTask, demographicsTask);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception has been thrown calling Emis.");
                }

                var result =
                    new EmisAppointmentSlotsResultBuilder(
                            _logger,
                            _appointmentSlotsResponseMapper,
                            metaTask,
                            slotTask,
                            practiceTask,
                            demographicsTask,
                            emisUserSession)
                        .Build();

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
