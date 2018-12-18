using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.Support;
using static NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.TppClient;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Session
{
    public interface ITppSessionMapper
    {
        Option<TppUserSession> Map(TppApiObjectResponse<AuthenticateReply> authenticateResponse,
            string odsCode, string nhsNumber);
    }
    
    public class TppSessionMapper : ITppSessionMapper
    {
        private readonly ILogger<TppSessionMapper> _logger;

        public TppSessionMapper(ILogger<TppSessionMapper> logger)
        {
            _logger = logger;
        }
        
        public Option<TppUserSession> Map(TppApiObjectResponse<AuthenticateReply> authenticateResponse, string odsCode, string nhsNumber)
        {
            if (!IsResponseValid(authenticateResponse))
            {
                return Option.None<TppUserSession>();
            }
            
            var suidHeader = GetSuidHeader(authenticateResponse);

            return Option.Some(new TppUserSession
            {
                Suid = suidHeader?.Value,
                OnlineUserId = authenticateResponse.Body.OnlineUserId,
                PatientId = authenticateResponse.Body.PatientId,
                OdsCode = odsCode,
                NhsNumber = nhsNumber
            });
        }
        
        private bool IsResponseValid(TppApiObjectResponse<AuthenticateReply> authenticateResponse)
        {
            var suidHeader = GetSuidHeader(authenticateResponse);

            return new ValidateAndLog(_logger)
                .IsNotNull(authenticateResponse, nameof(authenticateResponse))
                .IsNotNull(authenticateResponse?.Body, nameof(authenticateResponse.Body))
                .IsNotNullOrWhitespace(suidHeader?.Value, nameof(suidHeader.Value))
                .IsNotNullOrWhitespace(authenticateResponse?.Body?.OnlineUserId, nameof(authenticateResponse.Body.OnlineUserId))
                .IsNotNullOrWhitespace(authenticateResponse?.Body?.PatientId, nameof(authenticateResponse.Body.PatientId))
                .IsValid();
        }

        private static KeyValuePair<string, string>? GetSuidHeader(TppApiObjectResponse<AuthenticateReply> authenticateResponse)
        {
            return authenticateResponse?.Headers?.FirstOrDefault(h => "suid".Equals(h.Key, StringComparison.Ordinal));
        }
    }
}