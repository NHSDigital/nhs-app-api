namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public static class WebContainerExtensions
    {
        public static WebButton ContainingButtonWithText(this IWebContainer container, string text)
            => WebButton.WithText(container.ContainerInteractor, text);
        public static WebText ContainingTagWithText(this IWebContainer container, string tag, string text)
            => WebText.WithTagAndText(container.ContainerInteractor, tag, text);
    }
}