using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Repository;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.UsersApi.Repository
{
    internal class UserDeviceRepository : IUserDeviceRepository
    {
        private readonly ILogger<UserDeviceRepository> _logger;
        private readonly IRepository<UserDevice> _repository;
        private const string RecordName = nameof(UserDevice);

        public UserDeviceRepository
        (
            ILogger<UserDeviceRepository> logger,
            IRepository<UserDevice> repository
        )
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<RepositoryCreateResult<UserDevice>> Create(UserDevice userDevice)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(userDevice, nameof(userDevice), ThrowError)
                    .IsValid();

                return await _repository.Create(userDevice, RecordName);

            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309", Justification =
            "Method ‘CompareOrdinal’ is not supported in repository")]
        public async Task<RepositoryFindResult<UserDevice>> Find(string nhsLoginId, string deviceId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(deviceId, nameof(deviceId), ThrowError)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsValid();


                return await _repository.Find(d => d.NhsLoginId == nhsLoginId && d.DeviceId == deviceId,
                    RecordName);

            }
            finally
            {
                _logger.LogExit();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1309", Justification =
            "Method ‘CompareOrdinal’ is not supported in repository")]
        public async Task<RepositoryDeleteResult<UserDevice>> Delete(string nhsLoginId, string deviceId)
        {
            try
            {
                _logger.LogEnter();

                new ValidateAndLog(_logger)
                    .IsNotNull(deviceId, nameof(deviceId), ThrowError)
                    .IsNotNull(nhsLoginId, nameof(nhsLoginId), ThrowError)
                    .IsValid();

                return await _repository.Delete(d => d.NhsLoginId == nhsLoginId && d.DeviceId == deviceId,
                    RecordName);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}