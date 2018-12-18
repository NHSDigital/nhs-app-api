using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class OrganDonationService : IOrganDonationService
    {
        private readonly ILogger<OrganDonationService> _logger;
        private readonly IMapper<DemographicsResponse, OrganDonationRegistration> _demographicsRegistrationMapper;
        private readonly IMapper<OrganDonationRegistration, LookupRegistrationRequest> _lookupRegistrationRequestMapper;

        private readonly IMapper<OrganDonationRegistration, OrganDonationSuccessResponse<RegistrationLookupResponse>,
                OrganDonationRegistration> _registrationMapper;

        private readonly IOrganDonationClient _organDonationClient;

        public OrganDonationService(
            ILoggerFactory loggerFactory,
            IMapper<DemographicsResponse, OrganDonationRegistration> demographicsRegistrationMapper,
            IMapper<OrganDonationRegistration, OrganDonationSuccessResponse<RegistrationLookupResponse>,
                    OrganDonationRegistration> registrationMapper,
            IMapper<OrganDonationRegistration, LookupRegistrationRequest> lookupRegistrationRequestMapper,
            IOrganDonationClient organDonationClient)
        {
            _logger = loggerFactory.CreateLogger<OrganDonationService>();
            _demographicsRegistrationMapper = demographicsRegistrationMapper;
            _lookupRegistrationRequestMapper = lookupRegistrationRequestMapper;
            _registrationMapper = registrationMapper;
            _organDonationClient = organDonationClient;
        }

        public async Task<OrganDonationResult> GetOrganDonation(DemographicsResult myRecord, UserSession userSession)
        {
            if (!(myRecord is DemographicsResult.SuccessfullyRetrieved demographicsResult))
            {
                _logger.LogDebug("GP systems demographics record not successfully retrieved");
                return new OrganDonationResult.DemographicsRetrievalFailed();
            }

            var response = _demographicsRegistrationMapper.Map(demographicsResult.Response);
            var lookupRegistrationRequest = _lookupRegistrationRequestMapper.Map(response);

            try
            {
                _logger.LogDebug("Fetching existing organ donation record");
                var existingRegistrationRecord =
                    await _organDonationClient.PostLookup(lookupRegistrationRequest, userSession);
                
                if (existingRegistrationRecord.HasSuccessResponse)
                {
                    _logger.LogDebug("Found existing record");
                    response = _registrationMapper.Map(response, existingRegistrationRecord.Body);

                    return new OrganDonationResult.ExistingRegistration(response);
                }
                else
                {
                    switch (existingRegistrationRecord.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            _logger.LogDebug("Could not find an existing record");
                            break;
                        case HttpStatusCode.Conflict:
                            _logger.LogDebug("Conflict, there is more than one record");
                            return new OrganDonationResult.DuplicateRecord();
                        case HttpStatusCode.BadRequest:
                            _logger.LogDebug(
                                $"The organ donation request is invalid with message {JsonConvert.SerializeObject(lookupRegistrationRequest)}");
                            return new OrganDonationResult.BadSearchRequest();
                        case HttpStatusCode.RequestTimeout:
                            _logger.LogDebug("The organ donation search timed-out");
                            return new OrganDonationResult.SearchTimeout();
                        case HttpStatusCode.InternalServerError:
                            _logger.LogDebug("Something went wrong when retrieving organ donation record");
                            return new OrganDonationResult.SearchError();
                    }
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request to retrieve existing organ donation record");
                return new OrganDonationResult.SearchSystemUnavailable();
            }

            return new OrganDonationResult.NewRegistration(response);
        }
    }
}