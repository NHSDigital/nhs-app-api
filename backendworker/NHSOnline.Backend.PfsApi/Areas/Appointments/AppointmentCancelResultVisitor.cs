using System.Threading.Tasks;
 using Microsoft.AspNetCore.Http;
 using Microsoft.AspNetCore.Mvc;
 using NHSOnline.Backend.GpSystems.Appointments;
 using NHSOnline.Backend.GpSystems.Appointments.Models;
 using NHSOnline.Backend.Metrics;
 using NHSOnline.Backend.Support;

 namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentCancelResultVisitor : ResultVisitorBase, IAppointmentCancelResultVisitor<Task<IActionResult>>
    {
        private readonly AppointmentCancelRequest _request;
        private readonly IAnonymousMetricLogger _metricLogger;

        public AppointmentCancelResultVisitor(
            AppointmentCancelRequest request,
            IErrorReferenceGenerator errorReferenceGenerator,
            Supplier supplier,
            IAnonymousMetricLogger metricLogger)
            : base(errorReferenceGenerator, supplier)
        {
            _request = request;
            _metricLogger = metricLogger;
        }

        protected override ErrorCategory ErrorCategory => ErrorCategory.Appointments;

        public async Task<IActionResult> Visit(AppointmentCancelResult.Success result)
        {
            await _metricLogger.AppointmentCancelResult(new AppointmentMetricData(_request.SessionName, _request.SlotType, StatusCodes.Status204NoContent));
            return new NoContentResult();
        }

        public async Task<IActionResult> Visit(AppointmentCancelResult.BadRequest result)
        {
            return await BuildErrorResultTask(StatusCodes.Status400BadRequest);
        }

        public async Task<IActionResult> Visit(AppointmentCancelResult.AppointmentNotCancellable result)
        {
            return await BuildErrorResultTask(StatusCodes.Status409Conflict);
        }

        public async Task<IActionResult> Visit(AppointmentCancelResult.TooLateToCancel result)
        {
            return await BuildErrorResultTask(Constants.CustomHttpStatusCodes.Status461TooLate);
        }

        public async Task<IActionResult> Visit(AppointmentCancelResult.Forbidden result)
        {
            return await BuildErrorResultTask(StatusCodes.Status403Forbidden);
        }

        public async Task<IActionResult> Visit(AppointmentCancelResult.BadGateway result)
        {
            return await BuildErrorResultTask(StatusCodes.Status502BadGateway);
        }

        public async Task<IActionResult> Visit(AppointmentCancelResult.InternalServerError result)
        {
            return await BuildErrorResultTask(StatusCodes.Status500InternalServerError);
        }

        private async Task<IActionResult> BuildErrorResultTask(int statusCode)
        {
            await _metricLogger.AppointmentCancelResult(new AppointmentMetricData(_request.SessionName, _request.SlotType, statusCode));
            return BuildErrorResult(statusCode);
        }
    }
}
