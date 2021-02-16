using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal sealed class Interactor<TDriver, TElement> where TElement : IWebElement
    {
        private readonly TestLogs _logs;
        private readonly TDriver _driver;
        private readonly Func<By, TElement> _findElement;

        internal Interactor(
            TestLogs logs,
            TDriver driver,
            Func<By, TElement> findElement)
        {
            _logs = logs;
            _driver = driver;
            _findElement = findElement;
        }

        internal void ActOnElementContext(By @by, Action<ElementContext<TDriver, TElement>> action)
        {
            var retryUntil = DateTime.UtcNow.Add(ExtendedTimeout.Value);

            while (true)
            {
                try
                {
                    var element = _findElement(by);
                    action(new ElementContext<TDriver, TElement>(_driver, element));
                    return;
                }
                catch (StaleElementReferenceException e)
                {
                    _logs.Info("{0}: Retrying", e.Message);
                }
                catch (WebDriverException e) when (DateTime.UtcNow < retryUntil)
                {
                    _logs.Info("{0}: Retrying", e.Message);
                }
                catch (NoSuchElementException e)
                {
                    var message = $"No {typeof(TElement).Name} found matching {@by}\n{e.Message}";
                    _logs.Error($"{message}\n{e.StackTrace}");
                    throw new AssertFailedException(message, e);
                }
                catch (WebDriverException e)
                {
                    var message = $"Failed to act on {typeof(TElement).Name} matching {by}\n{e.Message}";
                    _logs.Error($"{message}\n{e.StackTrace}");
                    throw new AssertFailedException(message, e);
                }
            }
        }

        internal void ActOnDriver(Action<TDriver> action) => action(_driver);

        internal Interactor<TDriver, TElement> CreateContainedInteractor(By findContainerBy)
        {
            return new(
                _logs,
                _driver,
                findBy => (TElement) _findElement(findContainerBy).FindElement(findBy));
        }
    }
}