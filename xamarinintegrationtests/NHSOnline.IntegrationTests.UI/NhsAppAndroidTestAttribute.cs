using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.Native.Android;

namespace NHSOnline.IntegrationTests.UI
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NhsAppAndroidTestAttribute : TestMethodAttribute
    {
        internal AndroidBrowserStackCapability Capabilities { get; }

        public AndroidDevice AndroidDevice { get; set; } = AndroidDevice.Pixel3;

        public AndroidOSVersion OSVersion { get; set; } = AndroidOSVersion.Nine;

        public bool IsFlipbookTest { get; set; }

        public NhsAppAndroidTestAttribute(params AndroidBrowserStackCapability[] capabilities)
        {
            Capabilities = capabilities.Aggregate(
                AndroidBrowserStackCapability.None,
                (result, capability) => result | capability);
        }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var testName = DisplayName ?? testMethod.TestMethodName;
            var requestedCapabilities = GetRequestedCapabilities(testMethod);

            var testExecution = new AutomatedTestExecution<IAndroidDriverWrapper>(
                logs => new AndroidDriverWrapper(testName, logs, requestedCapabilities, AndroidDevice, OSVersion));

            var testExecutor = new TestExecutor(testName, testMethod, testExecution);

            return testExecutor.Execute();
        }

        private static AndroidBrowserStackCapability GetRequestedCapabilities(ITestMethod testMethod)
        {
            var testAttributes = testMethod.GetAttributes<NhsAppAndroidTestAttribute>(false);
            Assert.AreEqual(1, testAttributes.Length, "Expected exactly one NhsAppAndroidTestAttribute on the test");

            return testAttributes.Single().Capabilities;
        }
    }
}