using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public interface IOrganDonationValidationService
    {
        bool IsPutValid(OrganDonationRegistrationRequest request);
        bool IsPostValid(OrganDonationRegistrationRequest request);
        bool IsDeleteValid(OrganDonationWithdrawRequest request);
    }

    public class OrganDonationValidationService : IOrganDonationValidationService
    {
        private readonly ILogger<OrganDonationValidationService> _logger;

        public OrganDonationValidationService(ILogger<OrganDonationValidationService> logger)
        {
            _logger = logger;
        }

        public bool IsPutValid(OrganDonationRegistrationRequest request)
        {
            var isRequestValid = IsOrganDonationRegistrationRequestValid(request);

            var isIdentifierValid = new ValidateAndLog(_logger)
                .IsNotNull(request?.Registration?.Identifier, nameof(request.Registration.Identifier), ThrowError)
                .IsValid();

            return isRequestValid && isIdentifierValid;
        }

        public bool IsPostValid(OrganDonationRegistrationRequest request)
        {
            return IsOrganDonationRegistrationRequestValid(request);
        }

        private bool IsOrganDonationRegistrationRequestValid(OrganDonationRegistrationRequest request)
        {
            var isValid = new ValidateAndLog(_logger)
                .IsNotNull(request, nameof(request), ThrowError)
                .IsNotNull(request?.Registration, nameof(request.Registration), ThrowError)
                .IsNotNull(request?.AdditionalDetails, nameof(request.AdditionalDetails), ThrowError)
                .IsNotNull(request?.Registration?.AddressFull, nameof(request.Registration.AddressFull), ThrowError)
                .IsNotNull(request?.Registration?.DateOfBirth, nameof(request.Registration.DateOfBirth), ThrowError)
                .IsNotNull(request?.Registration?.NhsNumber, nameof(request.Registration.NhsNumber), ThrowError)
                .IsNotNull(request?.Registration?.Name, nameof(request.Registration.Name), ThrowError)
                .IsValid();

            if (request.Registration.Decision != Decision.OptIn)
            {
                return isValid;
            }

            return isValid && new ValidateAndLog(_logger)
                       .HasValue(request.Registration.FaithDeclaration, nameof(request.Registration.FaithDeclaration),
                           ThrowError)
                       .IsValid();
        }

        public bool IsDeleteValid(OrganDonationWithdrawRequest request)
        {
            return
                new ValidateAndLog(_logger)
                    .IsNotNull(request, nameof(request), ThrowError)
                    .IsNotNull(request?.Identifier, nameof(request.Identifier), ThrowError)
                    .IsNotNull(request?.AddressFull, nameof(request.AddressFull), ThrowError)
                    .IsNotNull(request?.DateOfBirth, nameof(request.DateOfBirth), ThrowError)
                    .IsNotNull(request?.NhsNumber, nameof(request.NhsNumber), ThrowError)
                    .IsNotNull(request?.Name, nameof(request.Name), ThrowError)
                    .IsNotNull(request?.WithdrawReasonId, nameof(request.WithdrawReasonId), ThrowError)
                    .IsValid();
        }
    }
}
