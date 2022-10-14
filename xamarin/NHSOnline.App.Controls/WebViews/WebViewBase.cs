using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Xamarin.Forms;

namespace NHSOnline.App.Controls.WebViews
{
    public abstract class WebViewBase : WebView
    {
        protected static JsonSerializerSettings Settings { get; } = CreateJsonSerializerSettings();

        protected static string ConvertToJsonString<T>(T value) => JsonConvert.SerializeObject(value, Settings);

        protected static T ConvertFromJsonString<T>(string json)
            => JsonConvert.DeserializeObject<T>(json, Settings) ?? throw new ArgumentException($"Failed to deserialise JSON to {typeof(T).FullName}", nameof(json));

        protected static JsonSerializerSettings CreateJsonSerializerSettings()
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