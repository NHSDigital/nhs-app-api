using EventKit;

namespace NHSOnline.App.iOS.DependencyServices.Calendar
{
    public class ApplicationEventStore
    {
        public static ApplicationEventStore Current { get; }
        public EKEventStore EventStore { get; }

        static ApplicationEventStore()
        {
            Current = new ApplicationEventStore();
        }

        private ApplicationEventStore()
        {
            EventStore = new EKEventStore();
        }
    }
}