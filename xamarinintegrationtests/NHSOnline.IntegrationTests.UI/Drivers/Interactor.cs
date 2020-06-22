using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal sealed class Interactor<TElement>
    {
        private readonly TestLogs _logs;
        private readonly Func<By, TElement> _findElement;

        internal Interactor(
            TestLogs logs,
            Func<By, TElement> findElement)
        {
            _logs = logs;
            _findElement = findElement;
        }

        internal void ActOnElement(By @by, Action<TElement> action)
        {
            var retryUntil = DateTime.UtcNow.Add(TimeSpan.FromSeconds(2));
            while (true)
            {
                try
                {
                    var element = _findElement(by);
                    action(element);
                    return;
                }
                catch (StaleElementReferenceException e) when (DateTime.UtcNow < retryUntil)
                {
                    _logs.Info("{0}: Retrying", e.Message);
                }
                catch (NoSuchElementException e)
                {
                    throw new AssertFailedException($"No {typeof(TElement).Name} found matching {by}", e);
                }
                catch (WebDriverException e)
                {
                    throw new AssertFailedException($"Failed to act on {typeof(TElement).Name} matching {by}", e);
                }
            }
        }
    }
}