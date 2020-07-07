using System;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IWebInteractor
    {
        internal void ActOnElement(By by, Action<IWebElement> action);
    }
}