using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class NhsLoginIdServiceTests
    {
        private const string ValidIdRange = "0123456789ABCDEF";

        private INhsLoginIdService _systemUnderTest;

        [TestMethod]
        public void WrapperService_CharactersIsNull_ThrowsError()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _systemUnderTest = new NhsLoginIdService(null));
        }

        [TestMethod]
        public void WrapperService_NhsLoginIdIsNull_ThrowsError()
        {
            _systemUnderTest = new NhsLoginIdService(ValidIdRange);

            Assert.ThrowsException<ArgumentNullException>(() => _systemUnderTest.HandlesNhsLoginId(null));
        }

        [TestMethod]
        public void WrapperService_NhsLoginIdIsBlank_ThrowsError()
        {
            _systemUnderTest = new NhsLoginIdService(ValidIdRange);

            Assert.ThrowsException<ArgumentNullException>(() => _systemUnderTest.HandlesNhsLoginId(string.Empty));
        }

        [DataTestMethod]
        [DataRow("", "6b6c1f3f-8697-43d3-8421-ee4ddecff12e", false)]
        [DataRow("abcdef", "6b6c1f3f-8697-43d3-8421-ee4ddecff12e", false)]
        [DataRow("123456", "6b6c1f3f-8697-43d3-8421-ee4ddecff12e", true)]
        [DataRow("", "1f2af4a2-14ff-4b3e-97f0-0b0a0ffd770c", false)]
        [DataRow("abcdef", "1f2af4a2-14ff-4b3e-97f0-0b0a0ffd770c", false)]
        [DataRow("123456", "1f2af4a2-14ff-4b3e-97f0-0b0a0ffd770c", true)]
        [DataRow("", "f12bcc11-f426-46c1-a4a9-93348445a1a9", false)]
        [DataRow("abcdef", "f12bcc11-f426-46c1-a4a9-93348445a1a9", true)]
        [DataRow("123456", "f12bcc11-f426-46c1-a4a9-93348445a1a9", false)]
        [DataRow("", "e72d1c90-c9f3-4d98-9da4-7a81ac6697a2", false)]
        [DataRow("abcdef", "e72d1c90-c9f3-4d98-9da4-7a81ac6697a2", true)]
        [DataRow("123456", "e72d1c90-c9f3-4d98-9da4-7a81ac6697a2", false)]
        public void WrapperService_HandlesNhsLoginId_ReturnsExpectedValues(string characters, string nhsLoginId, bool expected)
        {
            _systemUnderTest = new NhsLoginIdService(characters);

            var actual = _systemUnderTest.HandlesNhsLoginId(nhsLoginId);

            Assert.AreEqual(expected, actual);
        }
    }
}
