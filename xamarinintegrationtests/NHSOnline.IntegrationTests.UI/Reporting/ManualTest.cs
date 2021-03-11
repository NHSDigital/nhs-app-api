namespace NHSOnline.IntegrationTests.UI.Reporting
{
    public class ManualTest
    {
        public ManualTest(string zephyrId, string justification)
        {
            ZephyrId = zephyrId;
            Justification = justification;
        }

        public string ZephyrId { get; }
        public string Justification { get; }

    }
}