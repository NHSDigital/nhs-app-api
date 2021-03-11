using System;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal delegate void ActOnDriverAction<in TDriver, in TElement>(TDriver driver, Func<By, TElement> findElement);

    public interface IInteractor<out TDriver, out TElement>
    {
        internal void ActOnDriver(ActOnDriverAction<TDriver, TElement> action);
    }
}