using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    public class FakePrescriptionService : FakeServiceBase, IPrescriptionService
    {
        private readonly ILogger<FakePrescriptionService> _logger;

        public FakePrescriptionService(
            ILogger<FakePrescriptionService> logger,
            IFakeUserRepository fakeUserRepository)
            : base(logger, fakeUserRepository)
        {
            _logger = logger;
        }

        public async Task<GetPrescriptionsResult> GetPrescriptions(GpLinkedAccountModel gpLinkedAccountModel,
            DateTimeOffset? fromDate = null,  DateTimeOffset? toDate = null)
        {
            try
            {
                var fakeUser = await FindUser(gpLinkedAccountModel);
                return await fakeUser.PrescriptionAreaBehaviour.GetPrescriptions(gpLinkedAccountModel, fromDate, toDate);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<OrderPrescriptionResult> OrderPrescription(GpLinkedAccountModel gpLinkedAccountModel,
            RepeatPrescriptionRequest repeatPrescriptionRequest)
        {
            try
            {
                var fakeUser = await FindUser(gpLinkedAccountModel);
                return await fakeUser.PrescriptionAreaBehaviour.OrderPrescription(gpLinkedAccountModel,
                    repeatPrescriptionRequest);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}