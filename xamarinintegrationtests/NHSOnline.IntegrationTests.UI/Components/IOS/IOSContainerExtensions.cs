namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public static class IOSContainerExtensions
    {
        public static IOSLabel ContainingLabelWithText(this IIOSContainer container, string text)
            => IOSLabel.WithText(container.ContainerInteractor, text);

        public static IOSIcon ContainingIconWithDescription(this IIOSContainer container, string description)
            => IOSIcon.WithDescription(container.ContainerInteractor, description);

        public static IOSNavigationMenuItem ContainingMenuItemWithDescriptionAndText(this IIOSContainer container, string description, string text)
            => IOSNavigationMenuItem.WithIconDescriptionAndText(container.ContainerInteractor, description, text);
    }
}