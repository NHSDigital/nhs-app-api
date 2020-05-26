using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    public class FakeAppointmentsService : FakeServiceBase, IAppointmentsService
    {
        private readonly ILogger<FakeAppointmentsService> _logger;

        public FakeAppointmentsService(
            ILogger<FakeAppointmentsService> logger,
            IFakeUserRepository fakeUserRepository)
            : base(logger, fakeUserRepository)
        {
            _logger = logger;
        }

        public async Task<AppointmentBookResult> Book(GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentBookRequest request)
        {
            _logger.LogEnter();

            try
            {
                var fakeUser = FindUser(gpLinkedAccountModel);
                return await fakeUser.AppointmentsBehaviour.Book(gpLinkedAccountModel, request);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during building the response.");
                return await Task.FromResult<AppointmentBookResult>(new AppointmentBookResult.InternalServerError());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<AppointmentCancelResult> Cancel(GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentCancelRequest request)
        {
            _logger.LogEnter();

            try
            {
                var fakeUser = FindUser(gpLinkedAccountModel);
                return await fakeUser.AppointmentsBehaviour.Cancel(gpLinkedAccountModel, request);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during building the response.");
                return await Task.FromResult<AppointmentCancelResult>(new AppointmentCancelResult.InternalServerError());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<AppointmentsResult> GetAppointments(GpLinkedAccountModel gpLinkedAccountModel)
        {
            try
            {
                var fakeUser = FindUser(gpLinkedAccountModel);
                return await fakeUser.AppointmentsBehaviour.GetAppointments(gpLinkedAccountModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during building the response.");
                return await Task.FromResult<AppointmentsResult>(new AppointmentsResult.InternalServerError());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}