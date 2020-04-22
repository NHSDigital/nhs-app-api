using System;
using System.Net.Http;
using System.Text;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientRequestBuilder: ITppClientRequestBuilder, IDisposable
    {
        private const string RequestTypeHeader = "type";
        private const string RequestSuidHeader = "suid";

        private readonly HttpRequestMessage _request = new HttpRequestMessage { Method = HttpMethod.Post };

        private readonly TppConfigurationSettings _config;
        private readonly IGuidCreator _guidCreator;

        public TppClientRequestBuilder(
            TppConfigurationSettings config,
            IGuidCreator guidCreator)
        {
            _config = config;
            _guidCreator = guidCreator;
        }

        public string RequestType { get; private set; }
        public Guid Uuid { get; private set; }

        public ITppClientRequestBuilder Model<TRequest>(TRequest model) where TRequest : ITppRequest
        {
            RequestType = model.RequestType;
            Uuid = model.Uuid = _guidCreator.CreateGuid();

            model.ApiVersion = _config.ApiVersion;

            if (model is ITppApplicationRequest applicationRequest)
            {
                SetApplicationOnRequest(applicationRequest);
            }

            var xml = model.SerializeXml();

            _request.Content = new StringContent(xml, Encoding.UTF8, TppHttpClient.MediaType);
            _request.Headers.Add(RequestTypeHeader, model.RequestType);

            return this;

            void SetApplicationOnRequest(ITppApplicationRequest request)
            {
                request.Application = request.Application ?? new Application();
                request.Application.Name = _config.ApplicationName;
                request.Application.Version = _config.ApplicationVersion;
                request.Application.ProviderId = _config.ApplicationProviderId;
                request.Application.DeviceType = _config.ApplicationDeviceType;
            }
        }

        public ITppClientRequestBuilder Suid(string suid)
        {
            _request.Headers.Add(RequestSuidHeader, suid);
            return this;
        }

        public HttpRequestMessage Build() => _request;
        public void Dispose()
        {
            _request.Dispose();
        }
    }
}