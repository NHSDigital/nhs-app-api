using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers.Native.Android;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

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
                catch (WebDriverException e) when (DateTime.UtcNow < retryUntil)
                {
                    _logs.Info("{0}: Retrying", e.Message);
                }
                catch (NoSuchElementException e)
                {
                    _logs.Error(e.Message);
                    throw new AssertFailedException($"No {typeof(TElement).Name} found matching {by}", e);
                }
                catch (WebDriverException e)
                {
                    _logs.Error(e.Message);
                    throw new AssertFailedException($"Failed to act on {typeof(TElement).Name} matching {by}", e);
                }
            }
        }

        internal void AssertElementDoesntExist(By @by)
        {
            try
            {
                _findElement(by);
            }
            catch (NoSuchElementException e)
            {
                _logs.Info("Retrieved expected no such element exception", e.Message);
                return;
            }

            throw new AssertFailedException($"Was expecting not to find the element at {by}.");
        }

        public Interactor<TDriver, TElement> CreateContainedInteractor(By findContainerBy)
        {
            return new Interactor<TDriver, TElement>(
                _logs,
                _driver,
                findBy => (TElement) _findElement(findContainerBy).FindElement(findBy));
        }
    }
}