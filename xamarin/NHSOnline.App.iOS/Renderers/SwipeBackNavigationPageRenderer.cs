using NHSOnline.App.iOS.Renderers;
using NHSOnline.App.Navigation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(SwipeBackNavigationPageRenderer))]
namespace NHSOnline.App.iOS.Renderers
{
    internal sealed class SwipeBackNavigationPageRenderer : NavigationRenderer
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            InteractivePopGestureRecognizer.Delegate = new GestureDelegate(this, this);
        }

        private sealed class GestureDelegate : UIGestureRecognizerDelegate
        {
            private readonly UINavigationController _parent;
            private readonly NavigationRenderer _renderer;

            public GestureDelegate(UINavigationController parent, NavigationRenderer renderer)
            {
                _parent = parent;
                _renderer = renderer;
            }

            public override bool ShouldBegin(UIGestureRecognizer recognizer)
            {
                if (_renderer.Element is NavigationPage navigationPage &&
                    navigationPage.CurrentPage is ISwipeablePage page)
                {
                    if (!page.ShouldSwipeGoBack())
                    {
                        return false;
                    }
                }

                return _parent.ViewControllers?.Length > 1;
            }
        }
    }
}