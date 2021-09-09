using System;

namespace NHSOnline.App.NhsLogin
{
    public static class UrlExtensions
    {
        public static bool IsSameAddress(this Uri uri, Uri inputUri)
        {
            return uri.Scheme == inputUri.Scheme &&
                   uri.Host == inputUri.Host &&
                   uri.AbsolutePath == inputUri.AbsolutePath;
        }
    }
}