using System.Linq;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Configuration.Models
{
    public static class ValidateAndLogExtensions
    {
        public static ValidateAndLog KnownServicesConfigIsValid(
            this ValidateAndLog validateAndLog,
            IOptions<KnownServices> knownServicesOptions
        )
        {
            if (validateAndLog is null)
            {
                return null;
            }

            validateAndLog.IsNotNull(
                knownServicesOptions,
                nameof(knownServicesOptions),
                ValidateAndLog.ValidationOptions.ThrowError
            )
            .IsNotNull(
                knownServicesOptions?.Value,
                nameof(knownServicesOptions.Value),
                ValidateAndLog.ValidationOptions.ThrowError
            )
            .IsNotNull(
                knownServicesOptions?.Value?.Services,
                nameof(knownServicesOptions.Value.Services),
                ValidateAndLog.ValidationOptions.ThrowError
            );

            // validate root services
            knownServicesOptions?.Value
                ?.Services
                ?.ForEach(rs =>
                    validateAndLog.IsNotNull(
                        rs,
                        nameof(RootService),
                        ValidateAndLog.ValidationOptions.ThrowError
                    )
                    .IsNotEmpty(
                        rs?.Id,
                        nameof(RootService.Id),
                        ValidateAndLog.ValidationOptions.ThrowError
                    )
                    .IsNotNull(
                        rs?.Url,
                        nameof(RootService.Url),
                        ValidateAndLog.ValidationOptions.ThrowError
                    )
                );

            // validate all sub-services
            knownServicesOptions?.Value
                ?.Services
                ?.Where(rs => !(rs?.SubServices is null))
                .SelectMany(rs => rs.SubServices)
                .ForEach(ss =>
                    validateAndLog.IsNotNull(
                        ss,
                        nameof(SubService),
                        ValidateAndLog.ValidationOptions.ThrowError
                    )
                    .IsNotEmpty(
                        ss?.Path,
                        nameof(SubService.Path),
                        ValidateAndLog.ValidationOptions.ThrowError
                    )
                );

            return validateAndLog;
        }
    }
}
