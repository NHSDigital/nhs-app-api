using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.PfsApi.Messages.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.Messages
{
    public class MessagesResponse : ApiResponse
    {
        public string MessageId { get; }

        public MessagesResponse(HttpResponseMessage message, ILogger logger) : base(message.StatusCode)
        {
            if (!message.IsSuccessStatusCode)
            {
                return;
            }

            var responseBody = GetStringResponse(message, logger).Result;
            var response = JsonConvert.DeserializeObject<AddMessageResponse>(responseBody);
            MessageId = response?.MessageId;
        }

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();

        protected override bool FormatResponseIfUnsuccessful => false;
    }
}
