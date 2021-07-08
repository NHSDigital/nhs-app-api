namespace NHSOnline.App.iOS
{
    internal static class SecureScreen
    {
        public static void Show() => NhsApp.Current.MainPage.IsVisible = false;
        public static void Hide() => NhsApp.Current.MainPage.IsVisible = true;
    }
}