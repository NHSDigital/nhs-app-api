namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public static class WebContainerExtensions
    {
        public static WebButton ContainingButtonWithText(this IWebContainer container, string text)
            => WebButton.WithText(container.ContainerInteractor, text);
    }
}