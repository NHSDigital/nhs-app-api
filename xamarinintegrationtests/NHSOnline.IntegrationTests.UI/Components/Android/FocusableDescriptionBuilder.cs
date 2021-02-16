namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    internal sealed record FocusableDescriptionBuilder
    {
        public string Tag { get; init; } = string.Empty;
        public string Text { get; init;  } = string.Empty;

        public string Description => $"Tag: {Tag}; Text: {Text}";
    }
}