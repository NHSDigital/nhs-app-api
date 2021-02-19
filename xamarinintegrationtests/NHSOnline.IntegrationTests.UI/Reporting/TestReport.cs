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

        public string? BrowserStackSessionId { get; set; }

        public bool? ShouldRetry { get; set; }
        public string? RetryCategory { get; set; }

        public UnitTestOutcome? Outcome { get; set; }

        public string? TestFailureMessage { get; set; }

        public List<BusinessRule> BusinessRules { get; }

        internal void SetResult(TestResult testResult, RetryStatus retryStatus)
        {
            Outcome = testResult.Outcome;
            TestFailureMessage = testResult.TestFailureException?.Message;

            ShouldRetry = retryStatus.ShouldRetry;
            RetryCategory = retryStatus.Category;
        }
    }
}
