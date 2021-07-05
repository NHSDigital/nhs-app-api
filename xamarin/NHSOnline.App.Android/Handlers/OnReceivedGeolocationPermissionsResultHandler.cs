using System.Linq;
using Android;
using Android.Content.PM;
using Android.Webkit;

namespace NHSOnline.App.Droid.Handlers
{
    public sealed class OnReceivedGeolocationPermissionsResultHandler
    {
        public static OnReceivedGeolocationPermissionsResultHandler Instance { get;  } =
            new OnReceivedGeolocationPermissionsResultHandler();

        internal string? GeolocationOrigin { set; get; }

        internal GeolocationPermissions.ICallback? GeolocationCallback { set; get; }

        private OnReceivedGeolocationPermissionsResultHandler()
        {

        }

        internal void OnRequestPermissionsResult(int _, string[] permissions,
            Permission[] grantResults)
        {
            if (permissions.Contains(Manifest.Permission.AccessFineLocation) ||
                permissions.Contains(Manifest.Permission.AccessCoarseLocation))
            {
                var allow = grantResults[0] == Permission.Granted;
                GeolocationCallback?.Invoke(GeolocationOrigin, allow, false);
            }
        }
    }
}