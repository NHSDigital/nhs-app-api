namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class LoggedOutHomeScreenModel
    {
        internal LoggedOutHomeScreenStates ScreenState { get; }

        internal LoggedOutHomeScreenModel(LoggedOutHomeScreenStates screenStates = LoggedOutHomeScreenStates.Default)
        {
            ScreenState = screenStates;
        }
    }
}
