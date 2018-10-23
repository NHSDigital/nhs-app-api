using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Models.Appointments
{
    [TestClass]
    public class ListSlotsTests : XmlTestBase<ListSlots>
    {
        private const string PatientId = "Test patient id";
        private const string OnlineUserId = "Test online user id";
        private const string FromDate = "2018/12/25 08:30:00";
        private const string FromDateFormatted = "2018-12-25T08:30:00.0Z";
        private const string ToDate = "2018/12/31 17:30:00";
        private const string NumberOfDays = "6";

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
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider);

            var fromDateTIme = DateTime.Parse(FromDate, CultureInfo.InvariantCulture);
            var toDateTime = DateTime.Parse(ToDate, CultureInfo.InvariantCulture);

            var dateRange = new AppointmentSlotsDateRange(
                dateTimeOffsetProvider, 
                new DateTimeOffset(fromDateTIme),
                new DateTimeOffset(toDateTime)
                );

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
