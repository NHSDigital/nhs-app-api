using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.Areas.Configuration.Models;
using NHSOnline.Backend.PfsApi.Configuration;
using NHSOnline.Backend.PfsApi.Devices;

namespace NHSOnline.Backend.PfsApi.UnitTests.Configuration
{
    [TestClass]
    public class ConfigurationServiceTests
    {
        [TestMethod]
        public void GetConfiguration_Valid_ReturnsSuccess()
        {
            var settings = CreateSettings();
            var knownServices = CreateValidKnownServices();

            var systemUnderTest = CreateConfigurationService(knownServices, settings);

            var result = systemUnderTest.GetConfiguration();

            result.Should().NotBeNull();
            result.Response.Should().NotBeNull();
        }

        [TestMethod]
        public void GetConfiguration_DeviceSettingsNhsLoginPathNoPipes_ReturnsSinglePath()
        {
            var settings = CreateSettings(nhsLoginLoggedInPaths: "one/path");
            var knownServices = CreateValidKnownServices();

            var systemUnderTest = CreateConfigurationService(knownServices, settings);

            var result = systemUnderTest.GetConfiguration();

            result.Response.NhsLoginLoggedInPaths.Should().BeEquivalentTo("one/path");
        }

        [TestMethod]
        public void GetConfiguration_DeviceSettingsNhsLoginPathsSeparatedByPipes_ReturnsPaths()
        {
            var settings = CreateSettings(nhsLoginLoggedInPaths: "PathOne|PathTwo|PathThree");
            var knownServices = CreateValidKnownServices();

            var systemUnderTest = CreateConfigurationService(knownServices, settings);

            var result = systemUnderTest.GetConfiguration();

            result.Response.NhsLoginLoggedInPaths.Should().BeEquivalentTo("PathOne", "PathTwo", "PathThree");
        }

        [TestMethod]
        public void GetConfiguration_DeviceSettingsNhsLoginPathsSeparatedByPipes_TrimsReturnedPaths()
        {
            var settings = CreateSettings(nhsLoginLoggedInPaths: "  PathOne |PathTwo |   PathThree");
            var knownServices = CreateValidKnownServices();

            var systemUnderTest = CreateConfigurationService(knownServices, settings);

            var result = systemUnderTest.GetConfiguration();

            result.Response.NhsLoginLoggedInPaths.Should().BeEquivalentTo("PathOne", "PathTwo", "PathThree");
        }

        [DataTestMethod]
        [DataRow("1.34.2")]
        [DataRow("1.34.56")]
        public void GetConfiguration_Valid_ReturnsMinAndroidVersionFromDeviceSettings(string version)
        {
            var settings = CreateSettings(minimumSupportedAndroidVersion: version);
            var knownServices = CreateValidKnownServices();

            var systemUnderTest = CreateConfigurationService(knownServices, settings);

            var result = systemUnderTest.GetConfiguration();

            result.Response.MinimumSupportedAndroidVersion.Should().Be(version);
        }

        [DataTestMethod]
        [DataRow("1.34.2")]
        [DataRow("1.34.56")]
        public void GetConfiguration_Valid_ReturnsMiniOSVersionFromDeviceSettings(string version)
        {
            var settings = CreateSettings(minimumSupportediOSVersion: version);
            var knownServices = CreateValidKnownServices();

            var systemUnderTest = CreateConfigurationService(knownServices, settings);

            var result = systemUnderTest.GetConfiguration();

            result.Response.MinimumSupportediOSVersion.Should().Be(version);
        }

        [DataTestMethod]
        [DataRow("http://test.uri/")]
        [DataRow("http://another.test.uri/auth")]
        public void GetConfiguration_Valid_ReturnsFidoServerUrlFromDeviceSettings(string uri)
        {
            var settings = CreateSettings(fidoServerUrl: () => new Uri(uri));
            var knownServices = CreateValidKnownServices();

            var systemUnderTest = CreateConfigurationService(knownServices, settings);

            var result = systemUnderTest.GetConfiguration();

            result.Response.FidoServerUrl.Should().Be(new Uri(uri));
        }

        [TestMethod]
        public void GetConfiguration_SingleKnownService_ReturnsKnownService()
        {
            var settings = CreateSettings();
            var knownService = CreateKnownService();

            var knownServices = new KnownServices()
            {
                Services = new Dictionary<string, RootService>()
                {
                    { "MyId", knownService }
                }
            };

            var systemUnderTest = CreateConfigurationService(knownServices, settings);

            var result = systemUnderTest.GetConfiguration();

            result.Response.KnownServices
                .Should().ContainSingle()
                .Which.Should().BeSameAs(knownService);
        }

        [TestMethod]
        public void GetConfiguration_SingleKnownService_ReturnsKnownServiceWithIdSetFromDictionaryKey()
        {
            var settings = CreateSettings();
            var knownService = CreateKnownService();

            var knownServices = new KnownServices()
            {
                Services = new Dictionary<string, RootService>()
                {
                    { "MyId", knownService }
                }
            };

            var systemUnderTest = CreateConfigurationService(knownServices, settings);

            var result = systemUnderTest.GetConfiguration();

            result.Response.KnownServices
                .Should().ContainSingle()
                .Which.Id.Should().Be("MyId");
        }

        [TestMethod]
        public void GetConfiguration_MultipleKnownServices_ReturnsKnownServices()
        {
            var settings = CreateSettings();
            var knownServiceOne = CreateKnownService();
            var knownServiceTwo = CreateKnownService();
            var knownServiceThree = CreateKnownService();

            var knownServices = new KnownServices
            {
                Services = new Dictionary<string, RootService>
                {
                    { "MyId1", knownServiceOne },
                    { "MyId2", knownServiceTwo },
                    { "MyId3", knownServiceThree }
                }
            };

            var systemUnderTest = CreateConfigurationService(knownServices, settings);

            var result = systemUnderTest.GetConfiguration();

            result.Response.KnownServices
                .Should().BeEquivalentTo(new[] { knownServiceOne, knownServiceTwo, knownServiceThree });
        }

        [TestMethod]
        public void GetConfiguration_MultipleKnownServices_ReturnsKnownServicesWithIdsSetFromDictionaryKey()
        {
            var settings = CreateSettings();
            var knownServiceOne = CreateKnownService();
            var knownServiceTwo = CreateKnownService();
            var knownServiceThree = CreateKnownService();

            var knownServices = new KnownServices
            {
                Services = new Dictionary<string, RootService>
                {
                    { "MyId1", knownServiceOne },
                    { "MyId2", knownServiceTwo },
                    { "MyId3", knownServiceThree }
                }
            };

            var systemUnderTest = CreateConfigurationService(knownServices, settings);

            var result = systemUnderTest.GetConfiguration();

            result.Response.KnownServices.Should()
                .ContainSingle(ks => ks.Id == "MyId1")
                .And.ContainSingle(ks => ks.Id == "MyId2")
                .And.ContainSingle(ks => ks.Id == "MyId3");
        }

        [TestMethod]
        public void GetConfiguration_KnownServiceUrlContainsWebAppBaseUrl_UrlIsReplacedWithUrlFromDeviceSettings()
        {
            var configuredWebAppBaseUrl = new Uri("http://web.base.app/url");
            var settings = CreateSettings(webAppBaseUrl: () => configuredWebAppBaseUrl);
            var knownService = new RootService()
            {
                Url = new Uri("http://WebAppBaseUrl")
            };

            var knownServices = new KnownServices
            {
                Services = new Dictionary<string, RootService>
                {
                    { "MyId", knownService }
                }
            };

            var systemUnderTest = CreateConfigurationService(knownServices, settings);

            var result = systemUnderTest.GetConfiguration();

            result.Response.KnownServices.Should().ContainSingle()
                .Which.Url.Should().Be(configuredWebAppBaseUrl);
        }
        
        private ConfigurationService CreateConfigurationService(
            KnownServices knownServices,
            DeviceConfigurationSettings settings)
        {
            return new ConfigurationService(knownServices, settings);
        }

        private DeviceConfigurationSettings CreateSettings(
            string minimumSupportedAndroidVersion = "2.1.0",
            string minimumSupportediOSVersion = "3.5.0",
            string nhsLoginLoggedInPaths = "/path",
            Func<Uri> fidoServerUrl = null,
            Func<Uri> webAppBaseUrl = null)
        {
            fidoServerUrl ??= (() => new Uri("http://test.test.com"));
            webAppBaseUrl ??= (() => new Uri("http://test.test.com"));

            return new DeviceConfigurationSettings(
                nhsLoginLoggedInPaths,
                minimumSupportedAndroidVersion,
                minimumSupportediOSVersion,
                fidoServerUrl(),
                webAppBaseUrl());
        }

        private KnownServices CreateValidKnownServices()
        {
            return new KnownServices()
            {
                Services = new Dictionary<string, RootService>
                {
                    {"MyId", CreateKnownService()}
                }
            };
        }

        private static RootService CreateKnownService()
        {
            return new RootService {Url = new Uri("https://valid.service.url/")};
        }
    }
}
