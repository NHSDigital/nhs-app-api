namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    internal interface IAndroidLocatorStrategy
    {
        string Selector { get; }
        string Description { get; }
    }
}