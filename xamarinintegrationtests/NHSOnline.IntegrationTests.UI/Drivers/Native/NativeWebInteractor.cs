using NHSOnline.IntegrationTests.UI.Drivers.WebContext;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native
{
    internal sealed class NativeWebInteractor : IWebInteractor
    {
        private readonly NativeDriverContext _nativeDriverContext;
        private readonly IWebDriver _driver;
        private readonly IWebContextStrategy _webContextStrategy;
        private readonly Interactor<IWebDriver, IWebElement> _interactor;

        public NativeWebInteractor(
            NativeDriverContext nativeDriverContext,
            TestLogs logs,
            IWebDriver driver,
            IWebContextStrategy webContextStrategy)
            : this(nativeDriverContext, driver, webContextStrategy, new Interactor<IWebDriver, IWebElement>(logs, driver, driver.FindElement))
        {
        }

        private NativeWebInteractor(
            NativeDriverContext nativeDriverContext,
            IWebDriver driver,
            IWebContextStrategy webContextStrategy,
            Interactor<IWebDriver, IWebElement> interactor)
        {
            _nativeDriverContext = nativeDriverContext;
            _driver = driver;
            _webContextStrategy = webContextStrategy;
            _interactor = interactor;
        }

        void IInteractor<IWebDriver, IWebElement>.ActOnDriver(
            ActOnDriverAction<IWebDriver, IWebElement> action)
        {
            _webContextStrategy.SwitchTo();
            _interactor.ActOnDriver(action);
        }

        public IWebInteractor CreateContainedInteractor(By findBy)
                => new NativeWebInteractor(_nativeDriverContext, _driver, _webContextStrategy, _interactor.CreateContainedInteractor(findBy));

        public string ExecuteJavascript(string javascript)
        {
            _webContextStrategy.SwitchTo();
            return _driver.ExecuteJavaScript<string>(javascript);
        }
    }
}