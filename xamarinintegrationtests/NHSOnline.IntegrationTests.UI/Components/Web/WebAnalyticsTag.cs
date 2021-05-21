using NHSOnline.IntegrationTests.UI.Components.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebAnalyticsTag : IFocusable
    {
        private readonly string _tag;
        private readonly string _description;

        private WebAnalyticsTag(string tag, string description)
        {
            _tag = tag;
            _description = description;
        }

        public static WebAnalyticsTag WithTagAndDescription(string tag, string description)
            => new(tag, description);

        string IFocusable.ElementDescription
            => new FocusableDescriptionBuilder {Tag = _tag, Text = _description}.Description;
    }
}