using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Brothermailer.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Worker.Brothermailer
{
    public class BrothermailerClient: IBrothermailerClient
    {
        private readonly BrothermailerHttpClient _brothermailerHttpClient;
        private readonly ILogger<IBrothermailerClient> _logger;
        private readonly IBrothermailerConfig _brothermailerConfig;
        private const string BrothermailerPath = "signup.ashx";
        
        public BrothermailerClient(
            ILogger<BrothermailerClient> logger, 
            BrothermailerHttpClient brothermailerHttpClient, 
            IBrothermailerConfig brothermailerConfig)
        {
            _logger = logger;
            _brothermailerHttpClient = brothermailerHttpClient;
            _brothermailerConfig = brothermailerConfig;
        }
        
        public async Task<BrothermailerApiObjectResponse> SendEmailAddress(BrothermailerRequest brothermailerRequest)
        {            
            var brothermailerData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("userid", _brothermailerConfig.BrothermailerUserId),
                new KeyValuePair<string, string>(_brothermailerConfig.BrothermailerSig, ""),
                new KeyValuePair<string, string>("addressbookid", _brothermailerConfig.BrothermailerAddressBookId),
                new KeyValuePair<string, string>("ci_consenturl", ""),
                new KeyValuePair<string, string>("email", brothermailerRequest.EmailAddress),
                new KeyValuePair<string, string>("cd_ODSCODE", brothermailerRequest.OdsCode),
            };

            var brothermailerFormContent = new FormUrlEncodedContent(brothermailerData);
        
            return await Post(brothermailerFormContent,
                BrothermailerPath);            
        }
        
        private async Task<BrothermailerApiObjectResponse> Post(FormUrlEncodedContent brothermailerFormContent, string path)
        {
            var request = BuildBrothermailerRequest(HttpMethod.Post, path);
            
            request.Content = brothermailerFormContent;
            
            return await SendRequestAndParseResponse(request);
        }
        
        private static HttpRequestMessage BuildBrothermailerRequest(HttpMethod httpMethod, string path)
        {
            return new HttpRequestMessage(httpMethod, path);
        }
        
        private async Task<BrothermailerApiObjectResponse> SendRequestAndParseResponse(
            HttpRequestMessage request)
        {
            var responseMessage = await _brothermailerHttpClient.Client.SendAsync(request);
           
            var response = new BrothermailerApiObjectResponse(responseMessage.StatusCode);
            return response.Parse(responseMessage);
        }
        
        public abstract class BrothermailerApiReponse: ApiResponse
        {
            protected BrothermailerApiReponse(HttpStatusCode statusCode) :base(statusCode)
            {}

            public override bool HasSuccessResponse =>
                StatusCode.IsSuccessStatusCode() || StatusCode == HttpStatusCode.Redirect;
            
            public override string  ErrorForLogging => $"Error Code: '{StatusCode}'. ";
        }
        
        public class BrothermailerApiObjectResponse : BrothermailerApiReponse
        {
            public bool IsSuccess { get; set; }
            public bool IsInvalidEmail  { get; set; }
            
            public BrothermailerApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
            {}

            public BrothermailerApiObjectResponse Parse(
                HttpResponseMessage responseMessage)
            {
                return ParseLocationHeaders(responseMessage);
            }

            private BrothermailerApiObjectResponse ParseLocationHeaders(
                HttpResponseMessage responseMessage)
            {
                IsSuccess = responseMessage?.Headers?.Location?.ToString()?.Contains("result=success", 
                                StringComparison.InvariantCultureIgnoreCase) ?? false;
                IsInvalidEmail = responseMessage?.Headers?.Location?.ToString()?.Contains("reason=invalidemail", 
                                     StringComparison.InvariantCultureIgnoreCase) ?? false;
                return this;
            }

            protected override bool FormatResponseIfUnsuccessful => true;
        }
    }
}