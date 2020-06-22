using System;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace NHSOnline.App.DependencyInjection
{
    internal sealed class PageFactory : IPageFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PageFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Page CreatePageFor<TModel>(TModel model)
        {
            var pageFactory = _serviceProvider.GetRequiredService<IPageFactory<TModel>>();
            return pageFactory.CreatePage(model);
        }
    }

    internal sealed class PageFactory<TModel, TPage> : IPageFactory<TModel> where TPage : Page
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Action<TModel, TPage> _createPresenter;

        public PageFactory(
            IServiceProvider serviceProvider,
            Action<TModel, TPage> createPresenter)
        {
            _serviceProvider = serviceProvider;
            _createPresenter = createPresenter;
        }

        public Page CreatePage(TModel model)
        {
            var page = _serviceProvider.GetRequiredService<TPage>();
            _createPresenter(model, page);
            return page;
        }
    }
}