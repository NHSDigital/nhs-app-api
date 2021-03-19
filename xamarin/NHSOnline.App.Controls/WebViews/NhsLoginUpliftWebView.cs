using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Controls.WebViews.Payloads.Paycasso;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class NhsLoginUpliftWebView : WebView
    {
        public static readonly BindableProperty SelectMediaCommandProperty =
            BindableProperty.Create(nameof(SelectMediaCommandProperty), typeof(AsyncCommand<ISelectMediaRequest>), typeof(NhsLoginUpliftWebView));

        public static readonly BindableProperty LaunchPaycassoCommandProperty =
            BindableProperty.Create(nameof(LaunchPaycassoCommandProperty), typeof(AsyncCommand<LaunchPaycassoRequest>), typeof(NhsLoginUpliftWebView));

        private static JsonSerializerSettings Settings { get; } = CreateJsonSerializerSettings();

        public AsyncCommand<ISelectMediaRequest> SelectMediaCommand
        {
            get => (AsyncCommand<ISelectMediaRequest>) GetValue(SelectMediaCommandProperty);
            set => SetValue(SelectMediaCommandProperty, value);
        }

        public void LaunchPaycasso(string argumentJson)
        {
            var request = ConvertFromJsonString<LaunchPaycassoRequest>(argumentJson);
            LaunchPaycassoCommand.Execute(request);
        }

        public AsyncCommand<LaunchPaycassoRequest> LaunchPaycassoCommand
        {
            get => (AsyncCommand<LaunchPaycassoRequest>)GetValue(LaunchPaycassoCommandProperty);
            set => SetValue(LaunchPaycassoCommandProperty, value);
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