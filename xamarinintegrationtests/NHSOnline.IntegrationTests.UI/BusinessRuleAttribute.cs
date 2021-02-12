using System;

namespace NHSOnline.IntegrationTests.UI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class BusinessRuleAttribute: Attribute
    {
        public BusinessRuleAttribute(string id, string title)
        {
            Id = id;
            Title = title;
        }

        internal string Id { get; }
        internal string Title { get; }
    }
}
