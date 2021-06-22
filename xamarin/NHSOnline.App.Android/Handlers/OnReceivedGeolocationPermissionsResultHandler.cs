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

        internal string? MGeolocationOrigin { set; get; }

        internal GeolocationPermissions.ICallback? MGeoLocationCallback { set; get; }

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
                MGeoLocationCallback?.Invoke(MGeolocationOrigin, allow, false);
            }
        }
    }
}