using System;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Settings;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    internal sealed class ServiceDefinitionService: IServiceDefinitionService
    {
        private readonly ILogger<ServiceDefinitionService> _logger;

        private readonly FhirJsonSerializer _serializer;
        private readonly IAuditor _auditor;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IFhirParameterHelpers _fhirParameterHelpers;
        private readonly OnlineConsultationsProvidersSettings _olcProvidersSettings;
        private readonly IGuidCreator _guidCreator;
        private readonly ServiceDefinitionQuerySender _querySender;

        public ServiceDefinitionService(
            ILogger<ServiceDefinitionService> logger,
            IAuditor auditor,
            IGpSystemFactory gpSystemFactory,
            IFhirParameterHelpers fhirParameterHelpers,
            OnlineConsultationsProvidersSettings olcProvidersSettings,
            IGuidCreator guidCreator,
            ServiceDefinitionQuerySender querySender)
        {
            _logger = logger;
            _guidCreator = guidCreator;
            _querySender = querySender;

            _serializer = new FhirJsonSerializer();


            _auditor = auditor;

            _gpSystemFactory = gpSystemFactory;
            _fhirParameterHelpers = fhirParameterHelpers;

            _olcProvidersSettings = olcProvidersSettings;
        }

        public async Task<ServiceDefinitionResult> GetServiceDefinition(
            string providerKey,
            string serviceDefinitionId,
            string serviceDefinitionDescription,
            P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                var odsCode = userSession.OdsCode;

                var parameters = _fhirParameterHelpers.CreateInitialServiceDefinitionEvaluateParameters(odsCode);

                return await _querySender.SendEvaluateQueryAndHandleResponse(
                    providerKey,
                    serviceDefinitionId,
                    serviceDefinitionDescription,
                    JsonConvert.SerializeObject(parameters, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }),
                    false,
                    odsCode);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public ServiceDefinitionResult GetProviderName(string providerKey)
        {
            _logger.LogEnter();

            var provider = _olcProvidersSettings.Providers
                .FirstOrDefault(a => a.Provider.Equals(providerKey, StringComparison.Ordinal));

            if (provider is null)
            {
                return new ServiceDefinitionResult.NotFound();
            }

            _logger.LogExit();

            return new ServiceDefinitionResult.Success(provider.ProviderName);
        }

        public async Task<ServiceDefinitionResult> EvaluateServiceDefinition(
            string providerKey,
            string serviceDefinitionId,
            string serviceDefinitionDescription,
            Parameters parameters,
            bool addJavascriptDisabledHeader,
            bool demographicsConsentGiven,
            P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                if (parameters is null)
                {
                    _logger.LogError("Parameters can not be null");

                    return new ServiceDefinitionResult.BadRequest();
                }

                if (demographicsConsentGiven)
                {
                    // This code can be removed when the address can be retrieved from NHS login
                    if (userSession.GpUserSession != null)
                    {
                        var demographicsService = _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier)
                            .GetDemographicsService();

                        _logger.LogDebug("Fetching Demographics Address from GP system");
                        var result = await demographicsService.GetDemographics(
                            new GpLinkedAccountModel(userSession.GpUserSession));

                        if (result is DemographicsResult.Success demographicsResult)
                        {

                            parameters.Add("patient",
                                _fhirParameterHelpers.CreateFhirPatient(userSession, demographicsResult.Response.Address ??= ""));
                            await _auditor.Audit(
                                AuditingOperations.OnlineConsultationsDemographicAuditTypeRequest,
                                "User has agreed to share their name, age, NHS number and postal address.");
                        }
                        else
                        {
                            _logger.LogDebug("Fetching Demographics Address from GP system failed. Setting address to blank.");
                            parameters.Add("patient",
                                _fhirParameterHelpers.CreateFhirPatient(userSession, string.Empty));
                        }
                    }
                    else
                    {
                        _logger.LogDebug("Cannot fetch address from GP System. GP System unavailable. Setting address to blank.");
                        parameters.Add("patient",
                            _fhirParameterHelpers.CreateFhirPatient(userSession, string.Empty));
                    }
                }
                else
                {
                    await _auditor.Audit(
                        AuditingOperations.OnlineConsultationsDemographicAuditTypeRequest,
                        "User has not agreed to share their name, age, NHS number and postal address.");
                }

                return await _querySender.SendEvaluateQueryAndHandleResponse(
                    providerKey,
                    serviceDefinitionId,
                    serviceDefinitionDescription,
                    _serializer.SerializeToString(parameters),
                    addJavascriptDisabledHeader,
                    userSession.OdsCode,
                    GetSessionIdFromParameters(parameters));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<ServiceDefinitionIsValidResult> GetServiceDefinitionIsValid(string providerKey, P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                var odsCode = userSession.OdsCode;

                _logger.LogInformation($"Checking if online consultations are enabled for practice: {odsCode}");

                var requestId = _guidCreator.CreateGuid().ToString();

                _logger.LogInformation($"$isValid requestId: {requestId}");

                var parameters = _fhirParameterHelpers.CreateServiceDefinitionIsValidParameters(odsCode, requestId);

                return await _querySender.SendIsValidQueryAndHandleResponse(
                    providerKey,
                    _serializer.SerializeToString(parameters));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private ServiceDefinitionResult GetDemographicsErrorResult(DemographicsResult myRecord)
        {
            switch (myRecord)
            {
                case DemographicsResult.BadGateway _:
                    _logger.LogError("GP systems demographics call was unsuccessful");
                    return new ServiceDefinitionResult.DemographicsBadGateway();
                case DemographicsResult.Forbidden _:
                    _logger.LogError("GP systems demographics forbidden");
                    return new ServiceDefinitionResult.DemographicsForbidden();
                case DemographicsResult.InternalServerError _:
                    _logger.LogError("GP systems demographics threw an internal server error");
                    return new ServiceDefinitionResult.DemographicsInternalServerError();
                default:
                    _logger.LogError("GP systems demographics record not successfully retrieved");
                    return new ServiceDefinitionResult.DemographicsRetrievalFailed();
            }
        }

        private string GetSessionIdFromParameters(Parameters parameters)
        {
            try
            {
                return parameters?.Parameter?
                    .Where(p => "sessionId".Equals(p?.Name, StringComparison.Ordinal))
                    .Select(p => p?.Value)
                    .Cast<FhirString>()
                    .Select(p => p?.Value)
                    .FirstOrDefault();
            }
            catch (InvalidCastException e)
            {
                _logger.LogError(e, $"Parameter sessionId was not of expected type: {nameof(FhirString)}");
                return null;
            }
        }
    }
}