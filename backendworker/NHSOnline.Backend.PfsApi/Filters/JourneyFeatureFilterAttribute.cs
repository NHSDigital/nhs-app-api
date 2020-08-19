using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRules.Common;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class JourneyFeatureFilterAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly JourneyFeature _journeyFeature;
        private readonly HttpStatusCode _httpStatusCode;

        public JourneyFeatureFilterAttribute(JourneyFeature currentJourneyFeature, HttpStatusCode currentHttpStatusCode = HttpStatusCode.Forbidden)
        {
            _journeyFeature = currentJourneyFeature;
            _httpStatusCode = currentHttpStatusCode;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<JourneyFeatureFilterAttribute>>();
            var sjrClient = context.HttpContext.RequestServices.GetService<IServiceJourneyRulesClient>();
            var odsCode = context.HttpContext.RequestServices
                .GetRequiredService<IUserSessionService>()
                .GetRequiredUserSession<P9UserSession>()
                .OdsCode;

            ValidateObjects(logger, sjrClient, odsCode);

            var sjrResponse =  await sjrClient.GetServiceJourneyRules(odsCode);
            if (sjrResponse.HasSuccessResponse)
            {
                switch (_journeyFeature)
                {
                    case JourneyFeature.NominatedPharmacy:
                        if (sjrResponse.Body.Journeys.NominatedPharmacy != true)
                        {
                            logger.LogInformation($"Nominated Pharmacy is not enabled on SJR for OdsCode {odsCode}");
                            context.Result = new StatusCodeResult((int)_httpStatusCode);
                        }
                        break;

                    default:
                        logger.LogWarning($"{_journeyFeature} not handled by [{GetType().Name}]");
                        break;
                }
            }
            else
            {
                logger.LogInformation($"ServiceJourneyRules.Get returned a response code of {sjrResponse.StatusCode}");
                context.Result = new StatusCodeResult((int)sjrResponse.StatusCode);
            }
        }

        private static void ValidateObjects(
            ILogger<JourneyFeatureFilterAttribute> logger, IServiceJourneyRulesClient sjrClient, string odsCode)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (sjrClient == null)
            {
                throw new ArgumentNullException(nameof(sjrClient));
            }

            if (string.IsNullOrEmpty(odsCode))
            {
                throw new ArgumentNullException(nameof(odsCode));
            }
        }
    }
}