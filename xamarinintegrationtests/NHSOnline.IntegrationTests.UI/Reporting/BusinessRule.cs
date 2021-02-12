namespace NHSOnline.IntegrationTests.UI.Reporting
{
    internal sealed class BusinessRule
    {
        private BusinessRule(BusinessRuleAttribute attribute)
        {
            Id = attribute.Id;
            Title = attribute.Title;
        }

        public string Id { get; }
        public string Title { get; }

        internal static BusinessRule FromAttribute(BusinessRuleAttribute attribute) => new(attribute);
    }
}