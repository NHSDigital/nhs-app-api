using System;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace NHSOnline.App.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPageFactory(this IServiceCollection services)
        {
            return services.AddTransient<IPageFactory, PageFactory>();
        }

        public static IServiceCollection AddModelViewPresenter<TModel, TView, TPresenter>(
            this IServiceCollection services)
            where TView : Page
        {
            services.AddTransient<IPageFactory<TModel>, PageFactory<TModel, TView>>();
            services.AddTransient<TView>();

            var createPresenter = PresenterFactoryFactory.CreatePresenterFactoryMethod<TModel, TView, TPresenter>();

            return services.AddTransient<Action<TModel, TView>>(
                serviceProvider => (model, view) => _ = createPresenter(serviceProvider, model, view));
        }
    }
}