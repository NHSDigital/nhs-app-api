using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public static class ServiceProviderExtensions
    {
        public static T ResolveAreaBehaviour<T> (
            this IServiceProvider serviceProvider,
            Behaviour desiredBehaviour
        )
        {
            var areaType = typeof(T);
            var areaName = areaType.GetCustomAttributes<FakeGpAreaAttribute>()
                .FirstOrDefault()
                ?.AreaName;

            if (areaName is null)
            {
                throw new NotImplementedException(
                    $"Area type {areaType.FullName} does not have a " +
                    $"{nameof(FakeGpAreaAttribute)} attribute declared, this is required"
                );
            }

            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(typeof(ServiceProviderExtensions));

            logger.LogInformation($"Resolving implementation for {desiredBehaviour} behaviour in area: {areaName}");

            var areaTypeToBehaviourTypes =
                serviceProvider.GetService<IDictionary<Type, IDictionary<Behaviour, Type>>>();

            var behaviourTypes = areaTypeToBehaviourTypes[areaType];

            if (!behaviourTypes.ContainsKey(desiredBehaviour))
            {
                throw new NotImplementedException(
                    $"Area '{areaName}' does not have any implementations " +
                    $"for desired behaviour type: {desiredBehaviour}"
                );
            }

            logger.LogInformation($"Resolved implementation for {desiredBehaviour} behaviour in area: {areaName}");

            var resolvedBehaviourType = behaviourTypes[desiredBehaviour];

            if (!resolvedBehaviourType.GetInterfaces().Contains(areaType))
            {
                throw new InvalidOperationException(
                    $"Resolved behaviour does not match specified area '{areaName}', " +
                    "check service provider configuration"
                );
            }

            logger.LogInformation($"Creating instance of {desiredBehaviour} behaviour for area: {areaName}");

            return (T) serviceProvider?.GetService(resolvedBehaviourType);
        }
    }
}