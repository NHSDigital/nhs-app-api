using Foundation;

namespace NHSOnline.App.iOS.DependencyServices
{
    internal static class BiometricRegistrationDomainState
    {
        internal static void Set(NSData domainState)
        {
            using var key = new NSString("DomainState");
            NSUserDefaults.StandardUserDefaults.SetValueForKey(domainState, key);
        }

        internal static NSData Get()
        {
            return NSUserDefaults.StandardUserDefaults.DataForKey("DomainState");
        }

        internal static void Clear()
        {
            using var key = new NSString("DomainState");
            NSUserDefaults.StandardUserDefaults.RemoveObject(key);
        }
    }
}