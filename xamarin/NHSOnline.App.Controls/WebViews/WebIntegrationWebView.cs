using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NHSOnline.App.Controls.WebViews.Payloads;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class WebIntegrationWebView : WebView
    {
        public const string JavascriptObjectName = "nhsappNative";
        private static JsonSerializerSettings Settings { get; } = CreateJsonSerializerSettings();

        public static readonly BindableProperty RedirectToNhsAppPageCommandProperty =
            BindableProperty.Create(
                nameof(RedirectToNhsAppPageCommand),
                typeof(AsyncCommand<string>),
                typeof(WebIntegrationWebView));

        public void RedirectToNhsAppPage(string argument) => RedirectToNhsAppPageCommand.Execute(argument);

        public static readonly BindableProperty AddEventToCalendarCommandProperty =
            BindableProperty.Create(nameof(AddEventToCalendarCommand), typeof(AsyncCommand<AddEventToCalendarRequest>), typeof(WebIntegrationWebView));


        public AsyncCommand<string> RedirectToNhsAppPageCommand
        {
            get => (AsyncCommand<string>) GetValue(RedirectToNhsAppPageCommandProperty);
            set => SetValue(RedirectToNhsAppPageCommandProperty, value);
        }

        public void AddEventToCalendar(string json)
        {
            var request = ConvertFromJsonString<AddEventToCalendarRequest>(json);
            AddEventToCalendarCommand.Execute(request);
        }

        public AsyncCommand<AddEventToCalendarRequest> AddEventToCalendarCommand
        {
            get => (AsyncCommand<AddEventToCalendarRequest>) GetValue(AddEventToCalendarCommandProperty);
            set => SetValue(AddEventToCalendarCommandProperty, value);
        }

        private static T ConvertFromJsonString<T>(string json)
            => JsonConvert.DeserializeObject<T>(json, Settings) ?? throw new ArgumentException($"Failed to deserialise JSON to {typeof(T).FullName}", nameof(json));

        private static JsonSerializerSettings CreateJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
            return settings;
        }
    }
}
