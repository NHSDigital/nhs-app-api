namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    internal sealed record FocusableDescriptionBuilder
    {
        public string Tag { get; init; } = string.Empty;
        public string Text { get; init;  } = string.Empty;

        public string ContentDesc { get; init;  } = string.Empty;

        public string Description => $"Tag: {Tag}; Text: {Text}";

        public string ViewGroupDescription => $"Tag: {Tag}; content-desc: {ContentDesc}";
    }
}