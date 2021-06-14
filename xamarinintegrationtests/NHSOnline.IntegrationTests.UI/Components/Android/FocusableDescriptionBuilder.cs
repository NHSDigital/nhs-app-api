namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    internal sealed record FocusableDescriptionBuilder
    {
        public string Tag { get; init; } = string.Empty;
        public string Text { get; init;  } = string.Empty;
        public string Id { get; init; } = string.Empty;

        public string ContentDesc { get; init;  } = string.Empty;

        public string Description => $"Tag: {Tag}; Text: {Text}; Id: {Id}";

        public string ViewGroupDescription => $"Tag: {Tag}; Id: {Id}; content-desc: {ContentDesc}";
    }
}