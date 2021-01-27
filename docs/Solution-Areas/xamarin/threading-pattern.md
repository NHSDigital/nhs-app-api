# Xamarin Threading Pattern

## General Principle

> Most operating systems — including iOS, Android, and the Universal Windows Platform — use a single-threading model for code involving the user interface. This model is necessary to properly serialize user-interface events, including keystrokes and touch input. This thread is often called the main thread or the user-interface thread or the UI thread. The disadvantage of this model is that all code that accesses user interface elements must run on the application's main thread.
>
> -- <cite>https://docs.microsoft.com/en-us/xamarin/essentials/main-thread</cite>

As sumarised nicely in the above quote from the Xamarin documentation, all operations that affect the UI must be executed on the main thread.
The cited documentation suggests adding items of work into a queue to be executed on that main thread, however C# has a useful async threading model that we can use to our advantage here.
It's worth a brief understanding of the async/await threading model in C#, specifically synchronisation contexts, before continuing. These are nicely explained in [this Microsoft blog post](https://devblogs.microsoft.com/dotnet/configureawait-faq/)


## NHS App Pattern

Most of the things we do in the app are initiated by a user action such as clicking a button. The event handlers for such an action are executed on the main thread.
We can make use of the synchronisation contexts in order to execute code on a background thread and then return to the main thread automatically before updating the UI.

When you call an async method the compiler and IDE will helpfully warn you that you should "ConfigureAwait". This method name and its parameter are confusing to say the least, and so we've encapsulated them in more contextual extension methods:
* `PreserveThreadContext`
* `ResumeOnThreadPool`

We have also encapsulated the concept of running code on a background thread so as not to block the UI thread accidentally and thus create a poor user experience:
* `IBackgroundExecutionService`

In order to maintain a [consistent code base](../xamarin.md) and avoid any heavy brain activity deciding how best to use threads there is a simple set of rules that we follow.
The rules are:
* When invoking an async method from a [Presenter](model-view-presenter.md) always use `PreserveThreadContext`
* When invoking a service method from a [Presenter](model-view-presenter.md) always use `IBackgroundExecutionService` to ensure it runs on a background thread immediately
* When invoking an async method from a service always use `ResumeOnThreadPool`
* Never use `ConfigureAwait`

If the above rules are followed then what we see is any code that started on the main thread will resume on the main thread, and any services it calls will be executed on the thread pool.


## An example
 ``` csharp
private IBackgroundExecutionService _backgroundExecutionService;

// constructor that assigns the above omitted for brevity

private async void CreateSession()
{
    try
    {
        var createSessionResult = await _backgroundExecutionService.Run(TryCreateSession).PreserveThreadContext();
        await NavigateToLoggedInHomePage(createSessionResult).PreserveThreadContext();
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Failed to create session");
        await NavigateToFailedPage().PreserveThreadContext();
    }
}
 ```

This snippet of code uses an IBackgroundExecutionService to initiate some code on a thread pool but, crucially, uses `PreserveThreadContext` in order to be able to subsequently initiate a UI action of navigating to a new page.
