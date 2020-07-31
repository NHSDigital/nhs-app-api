using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
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
                var fakeUser = await FindUser(gpLinkedAccountModel);
                return await fakeUser.AppointmentsAreaBehaviour.Book(gpLinkedAccountModel, request);
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
                var fakeUser = await FindUser(gpLinkedAccountModel);
                return await fakeUser.AppointmentsAreaBehaviour.Cancel(gpLinkedAccountModel, request);
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
                var fakeUser = await FindUser(gpLinkedAccountModel);
                return await fakeUser.AppointmentsAreaBehaviour.GetAppointments(gpLinkedAccountModel);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}