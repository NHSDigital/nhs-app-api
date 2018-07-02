using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Extensions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
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
            try
            {
                var endUserSessionResponse = await _emisClient.SessionsEndUserSessionPost();
                if (!endUserSessionResponse.HasSuccessStatusCode)
                {
                    return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
                }

                var endUserSessionId = endUserSessionResponse.Body.EndUserSessionId;
                var sessionPostRequestModel = new SessionsPostRequest
                {
                    AccessIdentityGuid = connectionToken,
                    NationalPracticeCode = odsCode
                };

                var sessionsResponse = await _emisClient.SessionsPost(endUserSessionId, sessionPostRequestModel);
                if (!sessionsResponse.HasSuccessStatusCode)
                {
                    return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
                }

                var userPatientLinkToken = sessionsResponse.Body.ExtractUserPatientLinkToken();
                if (string.IsNullOrEmpty(userPatientLinkToken))
                {
                    return new Im1ConnectionVerifyResult.NotFound();
                }

                var demographicsResponse =
                    await _emisClient.DemographicsGet(userPatientLinkToken, sessionsResponse.Body.SessionId,
                        endUserSessionId);
                if (!demographicsResponse.HasSuccessStatusCode)
                {
                    return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
                }

                var nhsNumbers = demographicsResponse.Body.ExtractNhsNumbers();

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
            try
            {
                var endUserSessionResponse = await _emisClient.SessionsEndUserSessionPost();
                if (!endUserSessionResponse.HasSuccessStatusCode)
                {
                    return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                }

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
                
                var meApplicationsResponse = await _emisClient.MeApplicationsPost(endUserSessionId, meApplicationsPostRequest);
                
                var notFoundMessages = new[]
                {
                    EmisApiErrorMessages.MeApplicationsPost_AccountIdNotFound,
                    EmisApiErrorMessages.MeApplicationsPost_LinkageKeyDoesNotMatch,
                    EmisApiErrorMessages.MeApplicationsPost_SurnameOrDateOfBirthAreIncorrect
                };

                if (meApplicationsResponse.StatusCode == HttpStatusCode.Conflict ||
                    meApplicationsResponse.HasExceptionWithMessage(
                        EmisApiErrorMessages.MeApplicationsPost_AlreadyLinked))
                {
                    return new Im1ConnectionRegisterResult.AccountAlreadyExists();
                }

                if (meApplicationsResponse.StatusCode == HttpStatusCode.InternalServerError && 
                    meApplicationsResponse.HasExceptionWithAnyMessage(notFoundMessages))
                {
                    return new Im1ConnectionRegisterResult.NotFound();
                }

                if (meApplicationsResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    return new Im1ConnectionRegisterResult.BadRequest();
                }

                if (!meApplicationsResponse.HasSuccessStatusCode)
                {
                    return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                }

                var accessIdentityGuid = meApplicationsResponse.Body.AccessIdentityGuid;
                var sessionPostRequestModel = new SessionsPostRequest
                {
                    AccessIdentityGuid = accessIdentityGuid,
                    NationalPracticeCode = request.OdsCode
                };

                var sessionsResponse =
                    await _emisClient.SessionsPost(endUserSessionId, sessionPostRequestModel);
                if (!sessionsResponse.HasSuccessStatusCode)
                {
                    return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                }

                var userPatientLinkToken = sessionsResponse.Body.ExtractUserPatientLinkToken();
                if (string.IsNullOrEmpty(userPatientLinkToken))
                {
                    return new Im1ConnectionRegisterResult.NotFound();
                }

                var demographicsResponse =
                    await _emisClient.DemographicsGet(userPatientLinkToken, sessionsResponse.Body.SessionId,
                        endUserSessionId);
                if (!demographicsResponse.HasSuccessStatusCode)
                {
                    return new Im1ConnectionRegisterResult.SupplierSystemUnavailable();
                }

                var nhsNumbers = demographicsResponse.Body.ExtractNhsNumbers();

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
    }
}