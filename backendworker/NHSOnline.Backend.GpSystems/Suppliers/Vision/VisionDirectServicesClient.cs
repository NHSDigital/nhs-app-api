using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.ResponseParsers;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public class VisionDirectServicesClient : IVisionDirectServicesClient
    {
        private readonly VisionDirectServicesHttpClient _httpClient;
        private readonly ILogger<VisionDirectServicesClient> _logger;
        private readonly IXmlResponseParser _responseParser;
        private readonly string _providerId;

        private const string DirectServicesBasePath = "v1/organisations/{0}/onlineservices";

        private readonly string ConfigurationPath = $"{DirectServicesBasePath}/configuration";
        private readonly string DemographicsPath = $"{DirectServicesBasePath}/demographics";
        private readonly string ExistingAppointmentsPath = $"{DirectServicesBasePath}/existingAppointments";
        private readonly string EligibleRepeatsPath = $"{DirectServicesBasePath}/eligibleRepeats";

        public VisionDirectServicesClient(
            ILogger<VisionDirectServicesClient> logger,
            VisionDirectServicesHttpClient httpClient,
            IXmlResponseParser responseParser,
            VisionConfigurationSettings settings)
        {
            _logger = logger;
            _httpClient = httpClient;
            _responseParser = responseParser;
            _providerId = settings.ApplicationProviderId;
        }

        public async Task<VisionDirectServicesApiObjectResponse<PatientConfigurationResponse>> GetConfigurationV2(
            VisionConnectionToken token, string odsCode)
        {
            new ValidateAndLog(_logger).IsNotNullOrWhitespace(odsCode, nameof(odsCode), ThrowError)
                .IsNotNull(token, nameof(token), ThrowError)
                .IsNotNullOrWhitespace(token?.RosuAccountId, nameof(VisionConnectionToken.RosuAccountId), ThrowError)
                .IsNotNullOrWhitespace(token?.ApiKey, nameof(VisionConnectionToken.ApiKey), ThrowError)
                .IsValid();

            var path = string.Format(CultureInfo.InvariantCulture, ConfigurationPath, odsCode);

            var visionRequest = new VisionDirectServicesRequest(
                token.RosuAccountId,
                token.ApiKey,
                _providerId);

            return await Post<PatientConfigurationResponse>(visionRequest, path);
        }

        public async Task<VisionDirectServicesApiObjectResponse<PatientConfigurationResponse>> GetConfigurationV2(
            VisionUserSession userSession)
        {
            var path = string.Format(CultureInfo.InvariantCulture, ConfigurationPath, userSession.OdsCode);

            var visionRequest = new VisionDirectServicesRequest(
                userSession.RosuAccountId,
                userSession.ApiKey,
                _providerId);

            return await Post<PatientConfigurationResponse>(visionRequest, path);
        }

        public async Task<VisionDirectServicesApiObjectResponse<VisionDemographicsResponse>> GetDemographicsV2(VisionUserSession visionUserSession, DemographicsRequest requestContent)
        {
            var path = string.Format(CultureInfo.InvariantCulture, DemographicsPath, visionUserSession.OdsCode);

            var visionRequest = new VisionDirectServicesRequest(
                visionUserSession.RosuAccountId,
                visionUserSession.ApiKey,
                _providerId,
                visionUserSession.PatientId);

            return await Post<VisionDemographicsResponse>(visionRequest, path);
        }

        public async Task<VisionDirectServicesApiObjectResponse<BookedAppointmentsResponse>> GetExistingAppointmentsV2(VisionUserSession visionUserSession)
        {
            var path = string.Format(CultureInfo.InvariantCulture, ExistingAppointmentsPath, visionUserSession.OdsCode);

            var visionRequest = new VisionDirectServicesRequest(
                visionUserSession.RosuAccountId,
                visionUserSession.ApiKey,
                _providerId,
                visionUserSession.PatientId);

            return await Post<BookedAppointmentsResponse>(visionRequest, path);
        }

        public async Task<VisionDirectServicesApiObjectResponse<EligibleRepeatsResponse>> GetEligibleRepeatsV2(VisionUserSession visionUserSession)
        {
            var path = string.Format(CultureInfo.InvariantCulture, EligibleRepeatsPath, visionUserSession.OdsCode);

            var visionRequest = new VisionDirectServicesRequest(
                visionUserSession.RosuAccountId,
                visionUserSession.ApiKey,
                _providerId,
                visionUserSession.PatientId);

            return await Post<EligibleRepeatsResponse>(visionRequest, path);
        }

        private async Task<VisionDirectServicesApiObjectResponse<TResponse>> Post<TResponse>(VisionDirectServicesRequest model, string path)
        {
            using var request = BuildVisionRequest(HttpMethod.Post, path);

            var xmlNamespaces = new XmlSerializerNamespaces();
            xmlNamespaces.Add("vision", "urn:vision");
            var xml = model.SerializeXml(xmlNamespaces);
            request.Content = new StringContent(xml, Encoding.UTF8, "text/xml");

            return await SendRequestAndParseResponse<TResponse>(request);
        }

        private static HttpRequestMessage BuildVisionRequest(HttpMethod httpMethod, string path)
        {
            var request = new HttpRequestMessage(httpMethod, path);
            return request;
        }

        private async Task<VisionDirectServicesApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            _logger.LogEnter();

            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new VisionDirectServicesApiObjectResponse<TResponse>(responseMessage.StatusCode);

            await response.Parse(responseMessage, _responseParser, _logger);

            _logger.LogExit();

            return response;
        }
    }
}
