using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Models.Patient;
using NHSOnline.Backend.Worker.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.Suppliers.Emis
{
    public class EmisIm1ConnectionService : IIm1ConnectionService
    {
        private readonly IEmisClient _emisClient;

        public EmisIm1ConnectionService(IEmisClient emisClient)
        {
            _emisClient = emisClient;
        }

        public async Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode)
        {
            // TODO - handle case where user cannot access demographics, return Insufficient Access code.

            try
            {
                var endUserSessionResponse = await _emisClient.SessionsEndUserSessionPost();
                if (!endUserSessionResponse.HasSuccessStatusCode) { return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();}

                var endUserSessionId = endUserSessionResponse.Body.EndUserSessionId;
                var sessionPostRequestModel = new SessionsPostRequest
                {
                    AccessIdentityGuid = connectionToken,
                    NationalPracticeCode = odsCode
                };

                var sessionsResponse = await _emisClient.SessionsPost(endUserSessionId, sessionPostRequestModel);
                if (!sessionsResponse.HasSuccessStatusCode) { return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();}

                var userPatientLinkToken = sessionsResponse
                    ?.Body
                    ?.UserPatientLinks
                    ?.FirstOrDefault(x => x.AssociationType == AssociationType.Self)
                    ?.UserPatientLinkToken;

                if (userPatientLinkToken == null)
                {
                    return new Im1ConnectionVerifyResult.NotFound();
                }

                var demographicsResponse =
                    await _emisClient.DemographicsGet(userPatientLinkToken, sessionsResponse.Body.SessionId, endUserSessionId);
                if (!demographicsResponse.HasSuccessStatusCode) { return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();}

                var nhsNumbers = ExtractNhsNumbers(demographicsResponse.Body);

                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = connectionToken,
                    NhsNumbers = nhsNumbers
                };

                return new Im1ConnectionVerifyResult.SuccessfullyVerified(response);
            }
            catch (HttpRequestException)
            {
                return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
            }
        }

        public async Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request)
        {
            // TODO - handle the case where user cannot access demographics, return Insufficient Access code.

            try
            {
                var endUserSessionResponse = await _emisClient.SessionsEndUserSessionPost();
                if (!endUserSessionResponse.HasSuccessStatusCode) { return new Im1ConnectionRegisterResult.SupplierSystemUnavailable(); }

                var endUserSessionId = endUserSessionResponse.Body.EndUserSessionId;
                var meApplicationsPostRequest = new MeApplicationsPostRequest
                {
                    DateOfBirth = request.DateOfBirth,
                    Surname = request.Surname,
                    LinkageDetails = new LinkageDetails
                    {
                        AccountId = request.AccountId,
                        NationalPracticeCode = request.OdsCode,
                        LinkageKey = request.LinkageKey
                    }
                };

                var meApplicationsResponse =
                    await _emisClient.MeApplicationsPost(endUserSessionId, meApplicationsPostRequest);
                if (meApplicationsResponse.StatusCode == HttpStatusCode.Conflict ||
                    meApplicationsResponse.HasExceptionWithMessage(
                        EmisApiErrorMessages.MeApplicationsPost_AlreadyLinked))
                {
                    return new Im1ConnectionRegisterResult.AccountAlreadyExists();
                }
                if (!meApplicationsResponse.HasSuccessStatusCode) { return new Im1ConnectionRegisterResult.SupplierSystemUnavailable(); }

                var accessIdentityGuid = meApplicationsResponse.Body.AccessIdentityGuid;
                var sessionPostRequestModel = new SessionsPostRequest
                {
                    AccessIdentityGuid = accessIdentityGuid,
                    NationalPracticeCode = request.OdsCode
                };

                var sessionsResponse =
                    await _emisClient.SessionsPost(endUserSessionId, sessionPostRequestModel);
                if (!sessionsResponse.HasSuccessStatusCode) { return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();}

                var userPatientLinkToken = sessionsResponse
                    ?.Body
                    ?.UserPatientLinks
                    ?.FirstOrDefault(x => x.AssociationType == AssociationType.Self)
                    ?.UserPatientLinkToken;

                if (userPatientLinkToken == null)
                {
                    return new Im1ConnectionRegisterResult.NotFound();
                }

                var demographicsResponse =
                    await _emisClient.DemographicsGet(userPatientLinkToken, sessionsResponse.Body.SessionId, endUserSessionId);
                if (!demographicsResponse.HasSuccessStatusCode) { return new Im1ConnectionRegisterResult.SupplierSystemUnavailable(); }

                var nhsNumbers = ExtractNhsNumbers(demographicsResponse.Body);

                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = accessIdentityGuid,
                    NhsNumbers = nhsNumbers
                };

                return new Im1ConnectionRegisterResult.SuccessfullyRegistered(response);
            }
            catch (HttpRequestException)
            {
                return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
            }
        }

        private static IEnumerable<PatientNhsNumber> ExtractNhsNumbers(DemographicsGetResponse demographicsResponse)
        {
            var patientIdentifiers = demographicsResponse?.PatientIdentifiers;

            if (patientIdentifiers == null)
            {
                return Enumerable.Empty<PatientNhsNumber>();
            }

            return patientIdentifiers
                .Where(x => x.IdentifierType == IdentifierType.NhsNumber)
                .Select(x => new PatientNhsNumber { NhsNumber = x.IdentifierValue });
        }
    }
}