using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    public class FakeAppointmentSlotsService : FakeServiceBase, IAppointmentSlotsService
    {
        private readonly ILogger<FakeAppointmentSlotsService> _logger;

        public FakeAppointmentSlotsService(
            ILogger<FakeAppointmentSlotsService> logger,
            IFakeUserRepository fakeUserRepository)
            : base(logger, fakeUserRepository)
        {
            _logger = logger;
        }

        public async Task<AppointmentSlotsResult> GetSlots(GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentSlotsDateRange dateRange)
        {
            _logger.LogEnter();

            try
            {
                var fakeUser = await FindUser(gpLinkedAccountModel);
                return await fakeUser.AppointmentSlotsAreaBehaviour.GetSlots(gpLinkedAccountModel, dateRange);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}