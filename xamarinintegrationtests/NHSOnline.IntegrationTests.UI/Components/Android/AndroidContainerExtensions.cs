namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public static class AndroidContainerExtensions
    {
        public static AndroidLabel ContainingLabelWithText(this IAndroidContainer container, string text)
            => AndroidLabel.WithText(container.ContainerInteractor, text);

        public static AndroidIcon ContainingIconWithName(this IAndroidContainer container, string name)
            => AndroidIcon.WithName(container.ContainerInteractor, name);
    }
}