using System;
using FluentAssertions;
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

        internal void ActOnDriver(ActOnDriverAction<TDriver, TElement> action)
        {
            var retryUntil = DateTime.UtcNow.Add(ExtendedTimeout.Value);

            while (true)
            {
                By? attemptedBy = null;
                try
                {
                    action(
                        _driver,
                        by =>
                        {
                            attemptedBy = by;
                            return _findElement(by);
                        });
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
                catch (NoSuchElementException e) when (attemptedBy != null)
                {
                    var message = $"No {typeof(TElement).Name} found matching {attemptedBy}\n{e.Message}";
                    _logs.Error($"{message}\n{e.StackTrace}");
                    throw new AssertFailedException(message, e);
                }
                catch (WebDriverException e) when (attemptedBy != null)
                {
                    var message = $"Failed to act on {typeof(TElement).Name} matching {attemptedBy}\n{e.Message}";
                    _logs.Error($"{message}\n{e.StackTrace}");
                    throw new AssertFailedException(message, e);
                }
            }
        }

        internal void AssertElementCannotBeFound(By by, string because)
        {
            var retryUntil = DateTime.UtcNow.Add(ExtendedTimeout.Value);

            while (true)
            {
                try
                {
                    var element = _findElement(by);
                    element.Should().BeNull(because);
                }
                catch (StaleElementReferenceException e)
                {
                    _logs.Info("{0}: Retrying", e.Message);
                }
                catch (WebDriverException e) when (DateTime.UtcNow < retryUntil)
                {
                    _logs.Info("{0}: Retrying", e.Message);
                }
                catch (NoSuchElementException)
                {
                    return;
                }
                catch (WebDriverException e)
                {
                    var message = $"Failed to act on {typeof(TElement).Name} matching {by}\n{e.Message}";
                    _logs.Error($"{message}\n{e.StackTrace}");
                    throw new AssertFailedException(message, e);
                }
            }
        }
        
        internal Interactor<TDriver, TElement> CreateContainedInteractor(By findContainerBy)
        {
            return new(
                _logs,
                _driver,
                findBy => (TElement) _findElement(findContainerBy).FindElement(findBy));
        }
    }
}