using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public class EmisPatientMessageSendMapper : IEmisPatientMessageSendMapper
    {
        private readonly ILogger<EmisPatientMessageSendMapper> _logger;

        public EmisPatientMessageSendMapper(ILogger<EmisPatientMessageSendMapper> logger)
        {
            _logger = logger;
        }

        public Option<PostPatientMessageResponse> Map(MessagePostResponse response)
        {
            try
            {
                if (response.MessageSent != null && response.MessageSent == true)
                {
                    return Option.Some(new PostPatientMessageResponse
                    {
                        MessageSent = response.MessageSent
                    });
                }

                return Option.None<PostPatientMessageResponse>();
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, "Failed to map patient create message response from EMIS");
                return Option.None<PostPatientMessageResponse>();
            }
        }
    }
}