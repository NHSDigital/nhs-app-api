using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client.TppClient;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session
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
                PatientId = authenticateResponse.Body.User.Person.PatientId,
                OdsCode = odsCode,
                NhsNumber = nhsNumber,
                Name = authenticateResponse.Body.User?.Person?.PersonName?.Name
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
                .IsNotNullOrWhitespace(authenticateResponse?.Body?.User?.Person?.PatientId, nameof(authenticateResponse.Body.User.Person.PatientId))
                .IsValid();
        }

        private static KeyValuePair<string, string>? GetSuidHeader(TppApiObjectResponse<AuthenticateReply> authenticateResponse)
        {
            return authenticateResponse?.Headers?.FirstOrDefault(h => "suid".Equals(h.Key, StringComparison.Ordinal));
        }
    }
}