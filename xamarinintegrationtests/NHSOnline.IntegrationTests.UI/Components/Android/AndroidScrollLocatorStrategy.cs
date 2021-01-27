namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    internal sealed class AndroidScrollLocatorStrategy : IAndroidLocatorStrategy
    {
        private readonly IAndroidLocatorStrategy _wrappedLocatorStrategy;

        public AndroidScrollLocatorStrategy(IAndroidLocatorStrategy wrappedLocatorStrategy)
            => _wrappedLocatorStrategy = wrappedLocatorStrategy;

        public string Selector => $"new UiScrollable(new UiSelector().scrollable(true).instance(0)).scrollIntoView({_wrappedLocatorStrategy.Selector})";
        public string Description => _wrappedLocatorStrategy.Description;
    }
}