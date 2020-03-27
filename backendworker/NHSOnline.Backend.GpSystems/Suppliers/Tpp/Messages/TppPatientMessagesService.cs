using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Messages;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Messages
{
    internal sealed class TppPatientMessagesService: IPatientMessagesService
    {
        private readonly ILogger<TppPatientMessagesService> _logger;
        private readonly ITppClientRequest<TppUserSession, MessagesViewReply> _messagesViewPost;
        private readonly ITppPatientMessagesMapper _mapper;

        public TppPatientMessagesService(
            ILogger<TppPatientMessagesService> logger,
            ITppPatientMessagesMapper mapper,
            ITppClientRequest<TppUserSession, MessagesViewReply> messagesViewPost)
        {
            _logger = logger;
            _mapper = mapper;
            _messagesViewPost = messagesViewPost;
        }

        public async Task<GetPatientMessagesResult> GetMessages(GpUserSession gpUserSession)
        {
            _logger.LogEnter();

            var tppUserSession = (TppUserSession) gpUserSession;
            try
            {
                var response = await _messagesViewPost.Post(tppUserSession);
                if (response.HasSuccessResponse)
                {
                    _logger.LogInformation("Mapping TPP patient messages");
                    var mapped = _mapper.Map(response.Body);

                    if (mapped == null)
                    {
                        _logger.LogInformation("Mapping TPP patient messages returned null");
                        return new GetPatientMessagesResult.BadGateway();
                    }

                    _logger.LogInformation($"Number of TPP patient messages retrieved: {mapped.MessageSummaries.Count}");

                    return new GetPatientMessagesResult.Success(mapped);
                }

                if (response.HasErrorWithCode(TppApiErrorCodes.NoAccess))
                {
                    _logger.LogTppResponseAccessIsForbidden();
                    return new GetPatientMessagesResult.Forbidden();
                }

                _logger.LogTppErrorResponse(response);
                return new GetPatientMessagesResult.BadGateway();

            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Request to get patient messages was unsuccessful");
                return new GetPatientMessagesResult.BadGateway();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown error occured when getting patient messages");
                return new GetPatientMessagesResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public Task<GetPatientMessageResult> GetMessageDetails(string messageId, GpUserSession gpUserSession)
        {
            throw new NotImplementedException();
        }

        public Task<PutPatientMessageReadStatusResult> UpdateMessageMessageReadStatus(GpUserSession gpUserSession, UpdateMessageReadStatusRequestBody updateRequest)
        {
            throw new NotImplementedException();
        }

        public Task<GetPatientMessageRecipientsResult> GetMessageRecipients(GpUserSession gpUserSession)
        {
            throw new NotImplementedException();
        }

        public Task<PostPatientMessageResult> SendMessage(GpUserSession gpUserSession, CreatePatientMessage message)
        {
            throw new NotImplementedException();
        }

        public Task<DeletePatientMessageResult> DeleteMessage(GpUserSession gpUserSession, string messageId)
        {
            throw new NotImplementedException();
        }
    }


}