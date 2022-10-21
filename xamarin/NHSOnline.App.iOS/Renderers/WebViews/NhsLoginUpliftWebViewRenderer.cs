using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using WebKit;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(NhsLoginUpliftWebView), typeof(NhsLoginUpliftWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class NhsLoginUpliftWebViewRenderer : BaseWebViewRenderer
    {
        private readonly JavascriptBridge<NhsLoginUpliftWebView> _javascriptBridge;

        public NhsLoginUpliftWebViewRenderer() : this(CustomConfiguration)
        { }

        private NhsLoginUpliftWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            this.InstallIProov();

            config.MediaTypesRequiringUserActionForPlayback = WKAudiovisualMediaTypes.None;
            config.AllowsInlineMediaPlayback = true;

            _javascriptBridge = JavascriptBridge
                .ForWebView(() => (NhsLoginUpliftWebView)Element, "nativeNhsLogin")
                .Apply(config.UserContentController);
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