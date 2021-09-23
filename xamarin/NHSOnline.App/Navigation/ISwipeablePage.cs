namespace NHSOnline.App.Navigation
{
    public interface ISwipeablePage
    {
        /// <summary>
        /// Implement this method to provide behavior when the
        /// <c>UIKit.UINavigationController.InteractivePopGestureRecognizer</c> is triggered
        /// </summary>
        /// <returns>Whether or not the swipe back navigation was handled</returns>
        bool OnSwipeBack();
    }
}