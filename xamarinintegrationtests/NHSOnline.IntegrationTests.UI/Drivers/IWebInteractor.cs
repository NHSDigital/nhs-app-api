using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IWebInteractor: IInteractor<IWebDriver, IWebElement>
    {
        IWebInteractor CreateContainedInteractor(By findBy);
    }
}