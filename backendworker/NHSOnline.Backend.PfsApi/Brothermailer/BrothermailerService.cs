using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Brothermailer.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Brothermailer
{
    public class BrothermailerService: IBrothermailerService
    {
        private readonly ILogger<BrothermailerService> _logger;
        private readonly IBrothermailerClient _brothermailerClient;
        
        public BrothermailerService(ILogger<BrothermailerService> logger, IBrothermailerClient brothermailerClient)
        {
            _logger = logger;
            _brothermailerClient = brothermailerClient;
        }
        
        public async Task<BrothermailerResult> SendEmailAddress(BrothermailerRequest brothermailerRequest)
        {
            _logger.LogEnter();

            try
            {
                var isValid = new ValidateAndLog(_logger).IsNotNull(brothermailerRequest, nameof(brothermailerRequest))
                    .IsNotNullOrWhitespace(brothermailerRequest.EmailAddress, nameof(brothermailerRequest.EmailAddress))
                    .IsNotNullOrWhitespace(brothermailerRequest.OdsCode, nameof(brothermailerRequest.OdsCode))
                    .IsValid();

                if (!isValid)
                {
                    return new BrothermailerResult.BadRequest();
                }

                var response = await _brothermailerClient.SendEmailAddress(brothermailerRequest);

                if (!response.HasSuccessResponse)
                {
                    _logger.LogError($"Brothermailer service returned unsuccessful status code " +
                                     $"{response.StatusCode}");
                    return new BrothermailerResult.InternalServerError();
                }
                
                if (!response.IsSuccess)
                {
                    if (response.IsInvalidEmail)
                    {
                        _logger.LogError("Brothermailer service returned invalid email response");
                        return new BrothermailerResult.BadRequest();
                    }

                    _logger.LogError("Brothermailer service returned unsuccessful signup");
                    return new BrothermailerResult.InternalServerError();
                }

                return new BrothermailerResult.Success();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Brothermailer service is unavailable, error signing up");
                return new BrothermailerResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}