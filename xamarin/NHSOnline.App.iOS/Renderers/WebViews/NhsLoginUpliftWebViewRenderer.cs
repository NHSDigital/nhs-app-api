using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.DependencyServices;
using NHSOnline.App.iOS.Renderers.WebViews;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NhsLoginUpliftWebView), typeof(NhsLoginUpliftWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class NhsLoginUpliftWebViewRenderer : WkWebViewRenderer
    {
        private readonly JavascriptBridge<NhsLoginUpliftWebView> _javascriptBridge;

        public NhsLoginUpliftWebViewRenderer() : this(CustomConfiguration)
        {
        }

        private NhsLoginUpliftWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            this.InstallIProov();

            var startPaycassoCommand = new AsyncCommand<PaycassoData>(() => StartPaycassoHandler);

            _javascriptBridge = JavascriptBridge
                .ForWebView(() => (NhsLoginUpliftWebView)Element, "nativeNhsLogin")
                .AddFunction("startPaycasso", _ => startPaycassoCommand)
                .Apply(config.UserContentController);
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement == null)
            {
                NavigationDelegate = new WebViewNavigationDelegate(this);
            }

            base.OnElementChanged(e);
        }

        private async Task StartPaycassoHandler(PaycassoData data)
        {
#if SIMULATOR
            IPaycasso paycasso = new IosPaycassoSimulator();
#else
            IPaycasso paycasso = new IosPaycasso();
#endif

            var response = await paycasso.Launch(data).ConfigureAwait(true);

            var callbackMethod = response.Error switch
            {
                { } => "paycassoOnFailure",
                null => "paycassoOnSuccess"
            };

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            jsonSerializerSettings.Converters.Add(new StringEnumConverter());

            var json = JsonConvert.SerializeObject(response, jsonSerializerSettings);

            var webView = (NhsLoginUpliftWebView)Element;
            await webView.EvaluateJavaScriptAsync($"window.authentication.{callbackMethod}({json})").ConfigureAwait(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _javascriptBridge.Dispose();
            }

            base.Dispose(disposing);
        }

        private static WKWebViewConfiguration CustomConfiguration
            => new WebViewConfigurationBuilder().Build();
    }
}