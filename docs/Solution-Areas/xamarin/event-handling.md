# Xamarin Event Handling

## View => Presenter

As per the [Model View Presenter](model-view-presenter.md), the view notifies its presenter when a user interaction has occured that the application needs to respond to. The presenter is then responsible for the logic of how the application should respond (e.g. displaying the browser overly when the user clicks and external link).

### View => Presenter Pattern

The pattern is for the view to expose an asynchronous delegate for each event which the presenter may then assign its handler to. This is done by having a nested `IEvents` interface on the view itself.

``` csharp
    internal interface IExampleView: INavigationView<IExampleView.IEvents>
    {
        /* methods/properties the presenter can call/set on the view */

        internal interface IEvents
        {
            Func<Task>? EventWithoutArgs { get; set; }
            Func<EventInfo, Task>? EventWithArgs { get; set; }
        }
    }
```

The `INavigationView<>` interface exposes an `IAppNavigation<TEvents> AppNavigation` property which is used by the presenter to register event handlers and navigate to different pages within the app.

``` csharp
    internal sealed class ExamplePresenter
    {
        private readonly IExampleView _view;

        public ExamplePresenter(IGettingStartedView view)
        {
            _view = view;

            view.AppNavigation
                .RegisterHandler(EventWithoutArgsHandler, (v, h) => v.EventWithoutArgs = h)
                .RegisterHandler(EventWithArgsHandler, (v, h) => v.EventWithArgs = h);
        }

        private async Task EventWithoutArgsHandler()
        {
            // Handle event
        }

        private async Task EventWithoutArgsHandler(EventInfo args)
        {
            // Handle event
        }
    }
```

The view implements both the main view interface and the nested `IEvents` interface.

The event delegates are wrapped in an `AsyncCommand` instance to be executed. They can either be bound to a command on a control on the page or executed directly in a callback in the view. The `AsyncCommand` class provides resilience in the case of unhandled exceptions so it is essential all native callbacks/events are routed through an `AsyncCommand` as early as possible.

The `AppNavigation` class is used by the view to manage the registered handlers from the presenter. The handlers are enabled when the view is displayed and suspended when it is hidden. This prevents double tapping of UI elements causing unexpected behaviour.

``` csharp
    [DesignTimeVisible(false)]
    public partial class ExamplePage: IExampleView, IExampleView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IExampleView.IEvents> _appNavigation;

        public ExamplePage(ILogger<ExamplePage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<IExampleView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<IExampleView.IEvents> INavigationView<IExampleView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? EventWithoutArgs { get; set; }
        public ICommand EventWithoutArgsCommand => new AsyncCommand(() => EventWithoutArgs);

        public Func<EventInfo, Task>? EventWithArgsHandler { get; set; }
        public ICommand EventWithArgsHandlerCommand => new AsyncCommand(() => EventWithArgsHandler);

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();
        }

        protected override void OnDisappearing()
        {
            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();
        }
    }
```

### View => Presenter Rejected Patterns

#### View => Presenter C# Events

A more natural solution to this problem would be the use of C# events. The event handlers must return `void` and the recommended way to implement an asynchronous event handler is for it to be `async void`. As no task is returned it isn't possible to reliably catch unhandled exceptions thrown in the event handler. If the exception is thrown after an asynchronous call then it is captured by the task which there is no reference to.
