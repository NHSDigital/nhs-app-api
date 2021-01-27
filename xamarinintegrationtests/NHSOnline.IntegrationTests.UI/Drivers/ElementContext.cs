namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal sealed class ElementContext<TDriver, TElement>
    {
        internal ElementContext(TDriver driver, TElement element)
        {
            Driver = driver;
            Element = element;
        }

        internal TDriver Driver { get; }
        internal TElement Element { get; }
    }
}