using System;

namespace NHSOnline.App.Api.Client.Configuration
{
    internal sealed class GetConfigurationResponseValidator
        : IResponseModelValidator<GetConfigurationResponseModel, GetConfigurationResponse>
    {
        public ModelValidationResult<GetConfigurationResponse> Validate(GetConfigurationResponseModel model)
        {
            if (model.MinimumSupportedAndroidVersion == null || model.MinimumSupportediOSVersion == null)
            {
                return new ModelValidationResult<GetConfigurationResponse>.Invalid();
            }

            if (!Version.TryParse(model.MinimumSupportedAndroidVersion, out var androidVersion))
            {
                return new ModelValidationResult<GetConfigurationResponse>.Invalid();
            }

            if (!Version.TryParse(model.MinimumSupportediOSVersion, out var iosVersion))
            {
                return new ModelValidationResult<GetConfigurationResponse>.Invalid();
            }

            var response = new GetConfigurationResponse(androidVersion, iosVersion);

            return new ModelValidationResult<GetConfigurationResponse>.Valid(response);
        }
    }
}