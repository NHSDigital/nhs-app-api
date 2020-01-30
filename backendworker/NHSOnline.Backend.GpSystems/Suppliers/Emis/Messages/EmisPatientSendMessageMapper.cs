using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public class EmisPatientSendMessageMapper : IEmisPatientMessageSendMapper
    {
        private readonly ILogger<EmisPatientSendMessageMapper> _logger;

        public EmisPatientSendMessageMapper(ILogger<EmisPatientSendMessageMapper> logger)
        {
            _logger = logger;
        }

        public Option<PostMessageResponse> Map(MessagePostResponse response)
        {
            try
            {
                if (response.MessageSent != null && response.MessageSent == true)
                {
                    return Option.Some(new PostMessageResponse
                    {
                        MessageSent = response.MessageSent
                    });
                }

                return Option.None<PostMessageResponse>();
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, "Failed to map patient create message response from EMIS");
                return Option.None<PostMessageResponse>();
            }
        }
    }
}