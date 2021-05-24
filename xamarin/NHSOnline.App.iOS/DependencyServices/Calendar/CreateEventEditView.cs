using EventKitUI;

namespace NHSOnline.App.iOS.DependencyServices.Calendar
{
    public class CreateEventEditView : EKEventEditViewDelegate
    {
        private readonly EKEventEditViewController _eventController;

        public CreateEventEditView(EKEventEditViewController eventController)
        {
            _eventController = eventController;
        }

        public override void Completed(EKEventEditViewController controller, EKEventEditViewAction action)
        {
            _eventController.DismissViewController(true, null);
        }
    }
}