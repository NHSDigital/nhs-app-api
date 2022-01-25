using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.Native.IOS;

namespace NHSOnline.IntegrationTests.UI
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NhsAppIOSTestAttribute : TestMethodAttribute
    {
        internal IOSBrowserStackCapability Capabilities { get; }
        public IOSDevice IOSDevice { get; set; } = IOSDevice.iPhone12;

        public IOSVersion OSVersion { get; set; } = IOSVersion.Fourteen;

        public NhsAppIOSTestAttribute(params IOSBrowserStackCapability[] capabilities)
        {
            Capabilities = capabilities.Aggregate(
                IOSBrowserStackCapability.None,
                (result, capability) => result | capability);
        }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var testName = DisplayName ?? testMethod.TestMethodName;
            var requestedCapabilities = GetRequestedCapabilities(testMethod);

            var testExecution = new AutomatedTestExecution<IIOSDriverWrapper>(
                logs => new IOSDriverWrapper(testName, logs, requestedCapabilities, IOSDevice, OSVersion));

            var testExecutor = new TestExecutor(testName, testMethod, testExecution);

            return testExecutor.Execute();
        }

        private static IOSBrowserStackCapability GetRequestedCapabilities(ITestMethod testMethod)
        {
            var testAttributes = testMethod.GetAttributes<NhsAppIOSTestAttribute>(false);
            Assert.AreEqual(1, testAttributes.Length, "Expected exactly one NhsIOSTestAttribute on the test");

            return testAttributes.Single().Capabilities;
        }
    }
}