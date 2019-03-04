using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Models.Appointments
{
    [TestClass]
    public class ListSlotsTests : XmlTestBase<ListSlots>
    {
        private const string PatientId = "Test patient id";
        private const string OnlineUserId = "Test online user id";
        private const string FromDate = "2018/12/25 08:30:00";
        private const string FromDateFormatted = "2018-12-25T08:30:00.0Z";
        private const string ToDate = "2018/12/31 17:30:00";
        private const string NumberOfDays = "7";

        protected override ListSlots CreateModel()
        {
            var userSession = new TppUserSession
            {
                PatientId = PatientId,
                OnlineUserId = OnlineUserId
            };

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            var timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            var currentDateTimeProvider = new CurrentDateTimeProvider(timeZoneInfoProvider);
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider, currentDateTimeProvider);

            var fromDateTime = DateTime.Parse(FromDate, CultureInfo.InvariantCulture);
            var toDateTime = DateTime.Parse(ToDate, CultureInfo.InvariantCulture);

            var dateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider)
            {
                FromDate = fromDateTime,
                ToDate = toDateTime
            };

            return new ListSlots(userSession, dateRange);
        }

        [TestMethod]
        public void Serialization_PatientIdAttribute_HasCorrectValue()
        {
            Element.Attribute("patientId").Should().HaveValue(PatientId);
        }

        [TestMethod]
        public void Serialization_OnlineUserIdAttribute_HasCorrectValue()
        {
            Element.Attribute("onlineUserId").Should().HaveValue(OnlineUserId);
        }

        [TestMethod]
        public void Serialization_StartDateAttribute_HasCorrectValue()
        {
            Element.Attribute("startDate").Should().HaveValue(FromDateFormatted);
        }

        [TestMethod]
        public void Serialization_NumberOfDaysAttribute_HasCorrectValue()
        {
            Element.Attribute("numberOfDays").Should().HaveValue(NumberOfDays);
        }
    }
}
