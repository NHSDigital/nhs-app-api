using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientPracticeMessaging
{
    public class FakePatientMessagesService : FakeServiceBase, IPatientMessagesService
    {
        private readonly ILogger<FakePatientMessagesService> _logger;

        public FakePatientMessagesService(
            ILogger<FakePatientMessagesService> logger,
            IFakeUserRepository fakeUserRepository)
            : base(logger, fakeUserRepository)
        {
            _logger = logger;
        }


        public async Task<GetPatientMessagesResult> GetMessages(GpUserSession gpUserSession)
        {
            _logger.LogEnter();
            try
            {
                var fakeUser = await FindUser(gpUserSession.NhsNumber);
                return await fakeUser.PatientPracticeMessagingAreaBehaviour.GetMessages(gpUserSession);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<GetPatientMessageResult> GetMessageDetails(string messageId, GpUserSession gpUserSession)
        {
            _logger.LogEnter();
            try
            {
                var fakeUser = await FindUser(gpUserSession.NhsNumber);
                return await fakeUser.PatientPracticeMessagingAreaBehaviour.GetMessageDetails(messageId, gpUserSession);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<PutPatientMessageReadStatusResult> UpdateMessageMessageReadStatus(GpUserSession gpUserSession, UpdateMessageReadStatusRequestBody updateRequest)
        {
            _logger.LogEnter();
            try
            {
                var fakeUser = await FindUser(gpUserSession.NhsNumber);
                return await fakeUser.PatientPracticeMessagingAreaBehaviour.UpdateMessageMessageReadStatus(
                    gpUserSession, updateRequest);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<GetPatientMessageRecipientsResult> GetMessageRecipients(GpUserSession gpUserSession)
        {
            _logger.LogEnter();
            try
            {
                var fakeUser = await FindUser(gpUserSession.NhsNumber);
                return await fakeUser.PatientPracticeMessagingAreaBehaviour.GetMessageRecipients();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<PostPatientMessageResult> SendMessage(GpUserSession gpUserSession, CreatePatientMessage message)
        {
            _logger.LogEnter();
            try
            {
                var fakeUser = await FindUser(gpUserSession.NhsNumber);
                return await fakeUser.PatientPracticeMessagingAreaBehaviour.SendMessage(gpUserSession, message);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<DeletePatientMessageResult> DeleteMessage(GpUserSession gpUserSession, string messageId)
        {
            _logger.LogEnter();
            try
            {
                var fakeUser = await FindUser(gpUserSession.NhsNumber);
                return await fakeUser.PatientPracticeMessagingAreaBehaviour.DeleteMessage(gpUserSession, messageId);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}