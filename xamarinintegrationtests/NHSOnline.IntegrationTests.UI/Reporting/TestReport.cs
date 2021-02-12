using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.IntegrationTests.UI.Reporting
{
    internal sealed class TestReport
    {
        internal TestReport(ITestMethod testMethod)
        {
            ClassName = testMethod.TestClassName;
            MethodName = testMethod.TestMethodName;
            Description = testMethod.GetAttributes<DescriptionAttribute>(false).FirstOrDefault()?.Description;

            BusinessRules = testMethod.MethodInfo.DeclaringType?
                .GetCustomAttributes<BusinessRuleAttribute>()
                .Select(BusinessRule.FromAttribute)
                .ToList() ?? throw new InvalidOperationException($"Null declaring type for test method {testMethod.MethodInfo.Name}");
        }

        public string ClassName { get; }
        public string MethodName { get; }

        public string DisplayName => Regex.Replace(MethodName, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");

        public string? Description { get; }

        public List<BusinessRule> BusinessRules { get; }
    }
}
