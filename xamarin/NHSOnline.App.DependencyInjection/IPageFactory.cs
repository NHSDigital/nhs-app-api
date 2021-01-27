using Xamarin.Forms;

namespace NHSOnline.App.DependencyInjection
{
    public interface IPageFactory
    {
        Page CreatePageFor<TModel>(TModel model);
    }

    internal interface IPageFactory<TModel>
    {
        Page CreatePage(TModel model);
    }
}