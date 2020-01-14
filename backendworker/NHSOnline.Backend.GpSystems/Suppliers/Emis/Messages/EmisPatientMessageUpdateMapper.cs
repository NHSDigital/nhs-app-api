using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages
{
    public class EmisPatientMessageUpdateMapper : IEmisPatientMessageUpdateMapper
    {
        private readonly ILogger<EmisPatientMessageUpdateMapper> _logger;
        
        public EmisPatientMessageUpdateMapper(ILogger<EmisPatientMessageUpdateMapper> logger)
        {
            _logger = logger;
        }

        public PutPatientMessageUpdateStatusResponse Map(MessageUpdateResponse response)
        {
            try
            {
                return new PutPatientMessageUpdateStatusResponse
                {
                    MessageReadStateUpdateStatus = response.MessageReadStateUpdateStatus
                };
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, "Failed to map patient message read status update response from EMIS");
                return null;
            }
        }
    }
}