namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public static class AndroidContainerExtensions
    {
        public static AndroidLabel ContainingLabelWithText(this IAndroidContainer container, string text)
            => AndroidLabel.WithText(container.ContainerInteractor, text);

        public static AndroidIcon ContainingIconWithDescription(this IAndroidContainer container, string description)
            => AndroidIcon.WithDescription(container.ContainerInteractor, description);
    }
}