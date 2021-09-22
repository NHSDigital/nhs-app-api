using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Areas.LoggedOut.Presenters;
using NHSOnline.App.Areas.LoggedOut.Views;
using NHSOnline.App.DependencyInjection;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddLoggedOutArea(this IServiceCollection services)
        {
            return services
                .AddModelViewPresenter<BeginLoginModel, BeginLoginPage, BeginLoginPresenter>()
                .AddModelViewPresenter<BiometricLoginCouldNotLoginModel, BiometricLoginCouldNotLoginPage, BiometricLoginCouldNotLoginPresenter>()
                .AddModelViewPresenter<BiometricLoginFaceIdFailedModel, BiometricLoginFaceIdFailedPage, BiometricLoginFaceIdFailedPresenter>()
                .AddModelViewPresenter<BiometricLoginFaceIdLockedOutModel, BiometricLoginFaceIdLockedOutPage, BiometricLoginFaceIdLockedOutPresenter>()
                .AddModelViewPresenter<BiometricLoginFingerprintFailedModel, BiometricLoginFingerprintFailedPage, BiometricLoginFingerprintFailedPresenter>()
                .AddModelViewPresenter<BiometricLoginFingerprintLockedOutModel, BiometricLoginFingerprintLockedOutPage, BiometricLoginFingerprintLockedOutPresenter>()
                .AddModelViewPresenter<BiometricLoginLegacySensorNotValidModel, BiometricLoginLegacySensorNotValidPage, BiometricLoginLegacySensorNotValidPresenter>()
                .AddModelViewPresenter<BiometricLoginTouchIdFailedModel, BiometricLoginTouchIdFailedPage, BiometricLoginTouchIdFailedPresenter>()
                .AddModelViewPresenter<BiometricLoginTouchIdLockedOutModel, BiometricLoginTouchIdLockedOutPage, BiometricLoginTouchIdLockedOutPresenter>()
                .AddModelViewPresenter<GettingStartedModel, GettingStartedPage, GettingStartedPresenter>()
                .AddModelViewPresenter<UpdateRequiredModel, UpdateRequiredPage, UpdateRequiredPresenter>()
                .AddModelViewPresenter<UpdateCheckFailedModel, UpdateCheckFailedPage, UpdateCheckFailedPresenter>()
                .AddModelViewPresenter<CreateSessionErrorBadRequestModel, CreateSessionErrorBadRequestPage, CreateSessionErrorBadRequestPresenter>()
                .AddModelViewPresenter<CreateSessionErrorBadResponseFromUpstreamSystemModel, CreateSessionErrorBadResponseFromUpstreamSystemPage, CreateSessionErrorBadResponseFromUpstreamSystemPresenter>()
                .AddModelViewPresenter<CreateSessionErrorFailedAgeRequirementModel, CreateSessionErrorFailedAgeRequirementPage, CreateSessionErrorFailedAgeRequirementPresenter>()
                .AddModelViewPresenter<CreateSessionErrorFallbackModel, CreateSessionErrorFallbackPage, CreateSessionErrorFallbackPresenter>()
                .AddModelViewPresenter<CreateSessionErrorInternalServerErrorModel, CreateSessionErrorInternalServerErrorPage, CreateSessionErrorInternalServerErrorPresenter>()
                .AddModelViewPresenter<CreateSessionErrorOdsCodeNotSupportedModel, CreateSessionErrorOdsCodeNotSupportedPage, CreateSessionErrorOdsCodeNotSupportedPresenter>()
                .AddModelViewPresenter<CreateSessionErrorOdsCodeNotFoundModel, CreateSessionErrorOdsCodeNotFoundPage, CreateSessionErrorOdsCodeNotFoundPresenter>()
                .AddModelViewPresenter<CreateSessionErrorNoNhsNumberModel, CreateSessionErrorNoNhsNumberPage, CreateSessionErrorNoNhsNumberPresenter>()
                .AddModelViewPresenter<CreateSessionErrorUpstreamSystemTimeoutModel, CreateSessionErrorUpstreamSystemTimeoutPage, CreateSessionErrorUpstreamSystemTimeoutPresenter>()
                .AddModelViewPresenter<CreateSessionModel, CreateSessionPage, CreateSessionPresenter>()
                .AddModelViewPresenter<LoggedOutHomeScreenModel, LoggedOutHomeScreenPage, LoggedOutHomeScreenPresenter>()
                .AddModelViewPresenter<NhsLoginErrorModel, NhsLoginErrorPage, NhsLoginErrorPresenter>()
                .AddModelViewPresenter<NhsLoginModel, NhsLoginPage, NhsLoginPresenter>()
                .AddModelViewPresenter<NhsLoginTermsAndConditionsDeclinedModel, NhsLoginTermsAndConditionsDeclinedPage, NhsLoginTermsAndConditionsDeclinedPresenter>();
        }
    }
}