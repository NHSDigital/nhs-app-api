using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.Constants;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentBookResultVisitor : ResultVisitorBase, IAppointmentBookResultVisitor<Task<IActionResult>>
    {
        private readonly AppointmentBookRequest _request;
        private readonly IAnonymousMetricLogger _metricLogger;

        public AppointmentBookResultVisitor(AppointmentBookRequest request,
            IErrorReferenceGenerator errorReferenceGenerator,
            Supplier supplier,
            IAnonymousMetricLogger metricLogger)
            : base(errorReferenceGenerator, supplier)
        {
            _request = request;
            _metricLogger = metricLogger;
        }

        protected override ErrorCategory ErrorCategory => ErrorCategory.Appointments;

        public async Task<IActionResult> Visit(AppointmentBookResult.Success result)
        {
            await _metricLogger.AppointmentBookResult(new AppointmentMetricData(_request.SessionName, _request.SlotType, StatusCodes.Status201Created));
            return new CreatedResult(string.Empty, null);
        }

        public async Task<IActionResult> Visit(AppointmentBookResult.Forbidden result)
        {
            return await BuildErrorResultTask(StatusCodes.Status403Forbidden);
        }

        public async Task<IActionResult> Visit(AppointmentBookResult.SlotNotAvailable result)
        {
            return await BuildErrorResultTask(StatusCodes.Status409Conflict);
        }

        public async Task<IActionResult> Visit(AppointmentBookResult.BadGateway result)
        {
            return await BuildErrorResultTask(StatusCodes.Status502BadGateway);
        }

        public async Task<IActionResult> Visit(AppointmentBookResult.BadRequest result)
        {
            return await BuildErrorResultTask(StatusCodes.Status400BadRequest);
        }

        public async Task<IActionResult> Visit(AppointmentBookResult.AppointmentLimitReached result)
        {
            return await BuildErrorResultTask(CustomHttpStatusCodes.Status460LimitReached);
        }

        public async Task<IActionResult> Visit(AppointmentBookResult.InternalServerError result)
        {
            return await BuildErrorResultTask(StatusCodes.Status500InternalServerError);
        }

        private async Task<IActionResult> BuildErrorResultTask(int statusCode)
        {
            await _metricLogger.AppointmentBookResult(new AppointmentMetricData(_request.SessionName, _request.SlotType, statusCode));
            return BuildErrorResult(statusCode);
        }
    }
}