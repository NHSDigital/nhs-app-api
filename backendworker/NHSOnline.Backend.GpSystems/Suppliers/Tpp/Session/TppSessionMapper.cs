using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session
{
    public interface ITppSessionMapper
    {
        Option<TppUserSession> Map(TppApiObjectResponse<AuthenticateReply> authenticateResponse,
            string odsCode, string nhsNumber, IEnumerable<string> proxyPatientIdsToBeIncluded);

        Option<TppUserSession> Map(TppApiObjectResponse<AuthenticateReply> authenticateResponse,
            string odsCode, string nhsNumber, string patientId);
    }

    public class TppSessionMapper : ITppSessionMapper
    {
        private readonly ILogger<TppSessionMapper> _logger;

        public TppSessionMapper(ILogger<TppSessionMapper> logger)
        {
            _logger = logger;
        }

        public Option<TppUserSession> Map(TppApiObjectResponse<AuthenticateReply> authenticateResponse, string odsCode, string nhsNumber, IEnumerable<string> proxyPatientIdsToBeIncluded)
        {
            if (!IsResponseValid(authenticateResponse))
            {
                return Option.None<TppUserSession>();
            }

            var suidHeader = GetSuidHeader(authenticateResponse);

            var linkedPatients = authenticateResponse.Body.ExtractLinkedPatients()
                .Where(x => proxyPatientIdsToBeIncluded.Contains(x.PatientId));

            return Option.Some(new TppUserSession
            {
                Id = Guid.NewGuid(),
                Suid = suidHeader?.Value,
                OnlineUserId = authenticateResponse.Body.OnlineUserId,
                PatientId = authenticateResponse.Body.User.Person.PatientId,
                OdsCode = odsCode,
                NhsNumber = nhsNumber,
                Name = authenticateResponse.Body.User?.Person?.PersonName?.Name,
                ProxyPatients = linkedPatients.Select(x => new TppProxyUserSession
                {
                    Id = Guid.NewGuid(),
                    FullName = x.PersonName?.Name,
                    DateOfBirth = x.DateOfBirth,
                    NhsNumber = x.NationalId.Value,
                    PatientId = x.PatientId,
                }).ToList(),
            });
        }

        public Option<TppUserSession> Map(TppApiObjectResponse<AuthenticateReply> authenticateResponse, string odsCode, string nhsNumber, string patientId)
        {
            var response = Map(authenticateResponse, odsCode, nhsNumber, Enumerable.Empty<string>());

            if (response.HasValue)
            {
                response.ValueOrFailure().PatientId = patientId;
            }

            return response;
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