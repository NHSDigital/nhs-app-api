using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;

namespace NHSOnline.App.DependencyInjection
{
    internal static class PresenterFactoryFactory
    {
        private const BindingFlags ConstructorBindingFlags =
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        private static readonly MethodInfo GetRequiredServiceMethod = FindGetRequiredServiceMethod();

        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(PresenterFactoryFactory));

        internal static Func<IServiceProvider, TModel, TView, TPresenter> CreatePresenterFactoryMethod<TModel, TView, TPresenter>()
        {
            var presenterConstructor = GetConstructor<TPresenter>();

            var serviceProviderParameter = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
            var modelParameter = Expression.Parameter(typeof(TModel), "model");
            var viewParameter = Expression.Parameter(typeof(TView), "view");

            var callConstructor = Expression.New(presenterConstructor, presenterConstructor.GetParameters().Select(GetArgument));

            var createPresenter = Expression
                .Lambda<Func<IServiceProvider, TModel, TView, TPresenter>>(
                    callConstructor,
                    serviceProviderParameter,
                    modelParameter,
                    viewParameter)
                .Compile();

            return createPresenter;

            Expression GetArgument(ParameterInfo parameter)
            {
                if (parameter.ParameterType == typeof(TModel))
                {
                    return modelParameter;
                }

                if (parameter.ParameterType.IsAssignableFrom(typeof(TView)))
                {
                    return viewParameter;
                }

                var callServiceProviderGetRequiredService = Expression.Call(
                    null,
                    GetRequiredServiceMethod,
                    serviceProviderParameter,
                    Expression.Constant(parameter.ParameterType, typeof(Type)));

                return Expression.Convert(callServiceProviderGetRequiredService, parameter.ParameterType);
            }
        }

        private static ConstructorInfo GetConstructor<TPresenter>()
        {
            var constructors = typeof(TPresenter).GetConstructors(ConstructorBindingFlags);
            if (constructors.Length == 1)
            {
                return constructors[0];
            }

            var message = $"Presenter {typeof(TPresenter).Name} must have a single constructor";
            Logger.LogError(message);
            throw new InvalidOperationException(message);
        }

        private static MethodInfo FindGetRequiredServiceMethod()
        {
            var requiredServiceMethod = typeof(ServiceProviderServiceExtensions)
                .GetMethod(
                    nameof(ServiceProviderServiceExtensions.GetRequiredService),
                    new[] { typeof(IServiceProvider), typeof(Type) });

            if (requiredServiceMethod != null)
            {
                return requiredServiceMethod;
            }

            var message = "Missing " +
                          $"{nameof(ServiceProviderServiceExtensions)}.{nameof(ServiceProviderServiceExtensions.GetRequiredService)}" +
                          $"({nameof(IServiceProvider)}, {nameof(Type)})";
            Logger.LogError(message);
            throw new InvalidOperationException(message);
        }
    }
}