using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public interface IOrganDonationValidationService
    {
        bool IsPutValid(OrganDonationRegistrationRequest request, OrganDonationRegistration organDonationRegistration);
        bool IsPostValid(OrganDonationRegistrationRequest request, P9UserSession userSession);
        bool IsDeleteValid(OrganDonationWithdrawRequest request, OrganDonationRegistration organDonationRegistration);
    }

    public class OrganDonationValidationService : IOrganDonationValidationService
    {
        private readonly ILogger<OrganDonationValidationService> _logger;

        public OrganDonationValidationService(ILogger<OrganDonationValidationService> logger)
        {
            _logger = logger;
        }

        public bool IsPutValid(
            OrganDonationRegistrationRequest request,
            OrganDonationRegistration organDonationRegistration)
        {
            var isValid = new ValidateAndLog(_logger)
                .IsNotNull(request, nameof(request), ThrowError)
                .IsNotNull(request?.Registration, nameof(request.Registration), ThrowError)
                .IsNotNull(request?.AdditionalDetails, nameof(request.AdditionalDetails), ThrowError)
                .IsNotNull(request?.Registration?.AddressFull, nameof(request.Registration.AddressFull), ThrowError)
                .IsNotNull(request?.Registration?.DateOfBirth, nameof(request.Registration.DateOfBirth), ThrowError)
                .IsNotNull(request?.Registration?.NhsNumber, nameof(request.Registration.NhsNumber), ThrowError)
                .IsNotNull(request?.Registration?.Name, nameof(request.Registration.Name), ThrowError)
                .IsNotNull(request?.Registration?.Identifier, nameof(request.Registration.Identifier), ThrowError)
                .AreEqual(request?.Registration?.NhsNumber, organDonationRegistration?.NhsNumber, nameof(request.Registration.NhsNumber), ThrowError)
                .AreEqual(request?.Registration?.Identifier, organDonationRegistration?.Identifier, nameof(request.Registration.Identifier), ThrowError)
                .IsValid();

            if (request?.Registration?.Decision != Decision.OptIn)
            {
                return isValid;
            }

            return isValid && new ValidateAndLog(_logger)
                .HasValue(request.Registration.FaithDeclaration, nameof(request.Registration.FaithDeclaration),
                    ThrowError)
                .IsValid();
        }

        public bool IsPostValid(OrganDonationRegistrationRequest request, P9UserSession userSession)
        {
            var isValid = new ValidateAndLog(_logger)
                .IsNotNull(request, nameof(request), ThrowError)
                .IsNotNull(request?.Registration, nameof(request.Registration), ThrowError)
                .IsNotNull(request?.AdditionalDetails, nameof(request.AdditionalDetails), ThrowError)
                .IsNotNull(request?.Registration?.AddressFull, nameof(request.Registration.AddressFull), ThrowError)
                .IsNotNull(request?.Registration?.DateOfBirth, nameof(request.Registration.DateOfBirth), ThrowError)
                .IsNotNull(request?.Registration?.NhsNumber, nameof(request.Registration.NhsNumber), ThrowError)
                .IsNotNull(request?.Registration?.Name, nameof(request.Registration.Name), ThrowError)
                .AreEqual(request?.Registration?.NhsNumber, userSession?.NhsNumber, nameof(request.Registration.NhsNumber), ThrowError)
                .IsValid();

            if (request?.Registration?.Decision != Decision.OptIn)
            {
                return isValid;
            }

            return isValid && new ValidateAndLog(_logger)
                .HasValue(request.Registration.FaithDeclaration, nameof(request.Registration.FaithDeclaration),
                    ThrowError)
                .IsValid();
        }

        public bool IsDeleteValid(
            OrganDonationWithdrawRequest request,
            OrganDonationRegistration organDonationRegistration)
        {
            var isValid = new ValidateAndLog(_logger)
                    .IsNotNull(request, nameof(request), ThrowError)
                    .IsNotNull(request?.Identifier, nameof(request.Identifier), ThrowError)
                    .IsNotNull(request?.AddressFull, nameof(request.AddressFull), ThrowError)
                    .IsNotNull(request?.DateOfBirth, nameof(request.DateOfBirth), ThrowError)
                    .IsNotNull(request?.NhsNumber, nameof(request.NhsNumber), ThrowError)
                    .IsNotNull(request?.Name, nameof(request.Name), ThrowError)
                    .IsNotNull(request?.WithdrawReasonId, nameof(request.WithdrawReasonId), ThrowError)
                    .AreEqual(request?.NhsNumber, organDonationRegistration?.NhsNumber, nameof(request.NhsNumber), ThrowError)
                    .AreEqual(request?.Identifier, organDonationRegistration?.Identifier, nameof(request.Identifier), ThrowError)
                    .IsValid();

            return isValid;
        }
    }
}
