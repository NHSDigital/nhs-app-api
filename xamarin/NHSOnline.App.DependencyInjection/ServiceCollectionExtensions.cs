using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xamarin.Forms;

namespace NHSOnline.App.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        internal static ILogger Logger { get; set; } = null!;

        public static IServiceCollection AddPageFactory(this IServiceCollection services)
        {
            return services.AddTransient<IPageFactory, PageFactory>();
        }

        public static IServiceCollection AddModelViewPresenter<TModel, TView, TPresenter>(
            this IServiceCollection services)
            where TView : Page
        {
            Logger.LogDebug($"{nameof(AddModelViewPresenter)}<{{Model}}, {{View}}, {{Presenter}}>", typeof(TModel).Name, typeof(TView).Name, typeof(TPresenter).Name);

            services.AddTransient<IPageFactory<TModel>, PageFactory<TModel, TView>>();
            services.AddTransient<TView>();

            var createPresenter = PresenterFactoryFactory.CreatePresenterFactoryMethod<TModel, TView, TPresenter>();

            return services.AddTransient<Action<TModel, TView>>(
                serviceProvider => (model, view) => _ = createPresenter(serviceProvider, model, view));
        }
    }
}