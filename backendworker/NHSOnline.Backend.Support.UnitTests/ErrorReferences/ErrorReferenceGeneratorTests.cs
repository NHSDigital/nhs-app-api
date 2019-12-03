using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.Support.UnitTests
{
    [TestClass]
    public sealed class ErrorReferenceGeneratorTests: IDisposable
    {
        private ILoggerFactory _loggerFactory;
        private ILogger<ErrorReferenceGenerator> _logger;
        private RandomStringGenerator _randomStringGenerator;
        private IErrorReferenceGenerator _errorReferenceGenerator;
        private bool _disposed;
        
        private class DeterministicRandomGenerator : System.Security.Cryptography.RandomNumberGenerator
        {
            private readonly Random _random = new Random(0);
            public override void GetBytes(byte[] data)
            {
                _random.NextBytes(data);
            }
            public override void GetNonZeroBytes(byte[] data)
            {
                for (var i = 0; i < data.Length; i++)
                    data[i] = (byte)_random.Next(1, 256);
            }
        }
        
        [TestInitialize]
        public void TestInitialize()
        {
            _loggerFactory = new LoggerFactory();
            _logger = _loggerFactory.CreateLogger<ErrorReferenceGenerator>();
            _randomStringGenerator = new RandomStringGenerator(new DeterministicRandomGenerator());
            _errorReferenceGenerator = new ErrorReferenceGenerator(_logger, _randomStringGenerator);
        }

        [DataTestMethod]
        [DataRow(typeof(ErrorTypes.LoginBadRequest), "3a")]
        [DataRow(typeof(ErrorTypes.LoginForbidden), "3c")]
        [DataRow(typeof(ErrorTypes.LoginOdsCodeNotFoundOrNotSupported), "3f")]
        [DataRow(typeof(ErrorTypes.LoginMinimumAgeNotMet), "3g")]
        [DataRow(typeof(ErrorTypes.LoginUnexpectedError), "3h")]
        [DataRow(typeof(ErrorTypes.LoginBadGatewayEmis), "3e")]
        [DataRow(typeof(ErrorTypes.LoginBadGatewayTpp), "3t")]
        [DataRow(typeof(ErrorTypes.LoginBadGatewayMicrotest), "3m")]
        [DataRow(typeof(ErrorTypes.LoginBadGatewayVision), "3s")]
        [DataRow(typeof(ErrorTypes.LoginBadGatewayNhsLogin), "3n")]
        [DataRow(typeof(ErrorTypes.LoginServiceJourneyRulesOtherError), "3k")]
        [DataRow(typeof(ErrorTypes.AppointmentsBadRequest), "4a")]
        [DataRow(typeof(ErrorTypes.AppointmentsForbidden), "4c")]
        [DataRow(typeof(ErrorTypes.AppointmentsConflict), "4f")]
        [DataRow(typeof(ErrorTypes.AppointmentsLimitReached), "4g")]
        [DataRow(typeof(ErrorTypes.AppointmentsTooLateToCancel), "4h")]
        [DataRow(typeof(ErrorTypes.AppointmentsUnexpectedError), "4k")]
        [DataRow(typeof(ErrorTypes.AppointmentsBadGatewayEmis), "4e")]
        [DataRow(typeof(ErrorTypes.AppointmentsBadGatewayMicrotest), "4m")]
        [DataRow(typeof(ErrorTypes.AppointmentsBadGatewayTpp), "4t")]
        [DataRow(typeof(ErrorTypes.AppointmentsBadGatewayVision), "4s")]
        [DataRow(typeof(ErrorTypes.TimeoutEmis), "ze")]
        [DataRow(typeof(ErrorTypes.TimeoutMicrotest), "zm")]
        [DataRow(typeof(ErrorTypes.TimeoutNhsLogin), "zn")]
        [DataRow(typeof(ErrorTypes.TimeoutOrganDonation), "zo")]
        [DataRow(typeof(ErrorTypes.TimeoutServiceJourneyRules), "zj")]
        [DataRow(typeof(ErrorTypes.TimeoutTpp), "zt")]
        [DataRow(typeof(ErrorTypes.TimeoutVision), "zs")]
        [DataRow(typeof(ErrorTypes.PrescriptionsBadRequest), "5a")]
        [DataRow(typeof(ErrorTypes.PrescriptionsForbidden), "5c")]
        [DataRow(typeof(ErrorTypes.PrescriptionsConflict), "5f")]
        [DataRow(typeof(ErrorTypes.PrescriptionsMedicationAlreadyOrderedWithinLast30Days), "5g")]
        [DataRow(typeof(ErrorTypes.PrescriptionsUnexpectedError), "5k")]
        [DataRow(typeof(ErrorTypes.PrescriptionsBadGatewayEmis), "5e")]
        [DataRow(typeof(ErrorTypes.PrescriptionsBadGatewayMicrotest), "5m")]
        [DataRow(typeof(ErrorTypes.PrescriptionsBadGatewayTpp), "5t")]
        [DataRow(typeof(ErrorTypes.PrescriptionsBadGatewayVision), "5s")]
        [DataRow(typeof(ErrorTypes.PatientPracticeMessagesBadRequest), "9a")]
        [DataRow(typeof(ErrorTypes.PatientPracticeMessagesForbidden), "9c")]
        [DataRow(typeof(ErrorTypes.PatientPracticeMessagesUnexpectedError), "9k")]
        [DataRow(typeof(ErrorTypes.PatientPracticeMessagesBadGatewayEmis), "9e")]
        [DataRow(typeof(ErrorTypes.PatientPracticeMessagesBadGatewayMicrotest), "9m")]
        [DataRow(typeof(ErrorTypes.PatientPracticeMessagesBadGatewayTpp), "9t")]
        [DataRow(typeof(ErrorTypes.PatientPracticeMessagesBadGatewayVision), "9s")]
        public void GenerateAndLogErrorReference_UsingErrorType_ReturnsCorrectCode(Type type, string expectedPrefix)
        {
            var errorType = (ErrorTypes) Activator.CreateInstance(type);
            var errorCode = _errorReferenceGenerator.GenerateAndLogErrorReference(errorType);
            
            errorCode.Should().StartWith(expectedPrefix);
        }
        
        [DataTestMethod]
        [DataRow(ErrorCategory.Login, 400, "3a")]
        [DataRow(ErrorCategory.Login, 403, "3c")]
        [DataRow(ErrorCategory.Login, 464, "3f")]
        [DataRow(ErrorCategory.Login, 465, "3g")]
        [DataRow(ErrorCategory.Login, 500, "3h")]
        [DataRow(ErrorCategory.Appointments, 400, "4a")]
        [DataRow(ErrorCategory.Appointments, 403, "4c")]
        [DataRow(ErrorCategory.Appointments, 409, "4f")]
        [DataRow(ErrorCategory.Appointments, 460, "4g")]
        [DataRow(ErrorCategory.Appointments, 461, "4h")]
        [DataRow(ErrorCategory.Appointments, 500, "4k")]
        [DataRow(ErrorCategory.Prescriptions, 400, "5a")]
        [DataRow(ErrorCategory.Prescriptions, 403, "5c")]
        [DataRow(ErrorCategory.Prescriptions, 409, "5f")]
        [DataRow(ErrorCategory.Prescriptions, 466, "5g")]
        [DataRow(ErrorCategory.Prescriptions, 500, "5k")]
        [DataRow(ErrorCategory.PatientPracticeMessages, 400, "9a")]
        [DataRow(ErrorCategory.PatientPracticeMessages, 403, "9c")]
        [DataRow(ErrorCategory.PatientPracticeMessages, 500, "9k")]
        public void GenerateAndLogErrorReference_UsingErrorCategoryAndStatusCode_ReturnsCorrectCode(ErrorCategory errorCategory, int statusCode, string expectedPrefix)
        {
            var errorCode = _errorReferenceGenerator.GenerateAndLogErrorReference(errorCategory, statusCode);

            errorCode.Should().StartWith(expectedPrefix);
        }

        [DataTestMethod]
        [DataRow(ErrorCategory.Login, 400, "3a")]
        [DataRow(ErrorCategory.Login, 403, "3c")]
        [DataRow(ErrorCategory.Login, 464, "3f")]
        [DataRow(ErrorCategory.Login, 465, "3g")]
        [DataRow(ErrorCategory.Appointments, 400, "4a")]
        [DataRow(ErrorCategory.Appointments, 403, "4c")]
        [DataRow(ErrorCategory.Appointments, 409, "4f")]
        [DataRow(ErrorCategory.Appointments, 460, "4g")]
        [DataRow(ErrorCategory.Appointments, 461, "4h")]
        [DataRow(ErrorCategory.Appointments, 500, "4k")]
        [DataRow(ErrorCategory.Prescriptions, 400, "5a")]
        [DataRow(ErrorCategory.Prescriptions, 403, "5c")]
        [DataRow(ErrorCategory.Prescriptions, 409, "5f")]
        [DataRow(ErrorCategory.Prescriptions, 466, "5g")]
        [DataRow(ErrorCategory.Prescriptions, 500, "5k")]
        [DataRow(ErrorCategory.PatientPracticeMessages, 400, "9a")]
        [DataRow(ErrorCategory.PatientPracticeMessages, 403, "9c")]
        [DataRow(ErrorCategory.PatientPracticeMessages, 500, "9k")]
        public void GenerateAndLogErrorReference_UsingErrorCategoryAndStatusCodeAndOptionalSupplier_ReturnsCorrectCode(
            ErrorCategory errorCategory, int statusCode, string expectedPrefix)
        {
            // This test checks that we can optionally pass in a specific Source API for error codes that do not differ by supplier,
            //   to make life easier for the consuming code.
            var sourceApis = Enum.GetValues(typeof(SourceApi)).Cast<SourceApi>();
            foreach (var sourceApi in sourceApis)
            {
                var errorCode2 =
                    _errorReferenceGenerator.GenerateAndLogErrorReference(errorCategory, statusCode, sourceApi);
                errorCode2.Should().StartWith(expectedPrefix);
            }
        }

        [DataTestMethod]
        [DataRow(ErrorCategory.Login, 502, Supplier.Emis, "3e")]
        [DataRow(ErrorCategory.Login, 502, Supplier.Microtest, "3m")]
        [DataRow(ErrorCategory.Login, 502, Supplier.Tpp, "3t")]
        [DataRow(ErrorCategory.Login, 502, Supplier.Vision, "3s")]
        [DataRow(ErrorCategory.Appointments, 502, Supplier.Emis, "4e")]
        [DataRow(ErrorCategory.Appointments, 502, Supplier.Microtest, "4m")]
        [DataRow(ErrorCategory.Appointments, 502, Supplier.Tpp, "4t")]
        [DataRow(ErrorCategory.Appointments, 502, Supplier.Vision, "4s")]
        [DataRow(ErrorCategory.Prescriptions, 502, Supplier.Emis, "5e")]
        [DataRow(ErrorCategory.Prescriptions, 502, Supplier.Microtest, "5m")]
        [DataRow(ErrorCategory.Prescriptions, 502, Supplier.Tpp, "5t")]
        [DataRow(ErrorCategory.Prescriptions, 502, Supplier.Vision, "5s")]
        [DataRow(ErrorCategory.PatientPracticeMessages, 502, Supplier.Emis, "9e")]
        [DataRow(ErrorCategory.PatientPracticeMessages, 502, Supplier.Microtest, "9m")]
        [DataRow(ErrorCategory.PatientPracticeMessages, 502, Supplier.Tpp, "9t")]
        [DataRow(ErrorCategory.PatientPracticeMessages, 502, Supplier.Vision, "9s")]
        [DataRow(ErrorCategory.Timeout, 504, Supplier.Emis, "ze")]
        [DataRow(ErrorCategory.Timeout, 504, Supplier.Microtest, "zm")]
        [DataRow(ErrorCategory.Timeout, 504, Supplier.Tpp, "zt")]
        [DataRow(ErrorCategory.Timeout, 504, Supplier.Vision, "zs")]
        public void GenerateAndLogErrorReference_UsingErrorCategoryAndStatusCodeAndSupplier_ReturnsCorrectCode(ErrorCategory errorCategory, int statusCode, Supplier supplier, string expectedCode)
        {
            var errorCode = _errorReferenceGenerator.GenerateAndLogErrorReference(errorCategory, statusCode, supplier);

            errorCode.Should().StartWith(expectedCode);
        }
        
        [DataTestMethod]
        [DataRow(ErrorCategory.Login, 502, SourceApi.Emis, "3e")]
        [DataRow(ErrorCategory.Login, 502, SourceApi.Microtest, "3m")]
        [DataRow(ErrorCategory.Login, 502, SourceApi.NhsLogin, "3n")]
        [DataRow(ErrorCategory.Login, 502, SourceApi.Tpp, "3t")]
        [DataRow(ErrorCategory.Login, 502, SourceApi.Vision, "3s")]
        [DataRow(ErrorCategory.Appointments, 502, SourceApi.Emis, "4e")]
        [DataRow(ErrorCategory.Appointments, 502, SourceApi.Microtest, "4m")]
        [DataRow(ErrorCategory.Appointments, 502, SourceApi.Tpp, "4t")]
        [DataRow(ErrorCategory.Appointments, 502, SourceApi.Vision, "4s")]
        [DataRow(ErrorCategory.Prescriptions, 502, SourceApi.Emis, "5e")]
        [DataRow(ErrorCategory.Prescriptions, 502, SourceApi.Microtest, "5m")]
        [DataRow(ErrorCategory.Prescriptions, 502, SourceApi.Tpp, "5t")]
        [DataRow(ErrorCategory.Prescriptions, 502, SourceApi.Vision, "5s")]
        [DataRow(ErrorCategory.PatientPracticeMessages, 502, SourceApi.Emis, "9e")]
        [DataRow(ErrorCategory.PatientPracticeMessages, 502, SourceApi.Microtest, "9m")]
        [DataRow(ErrorCategory.PatientPracticeMessages, 502, SourceApi.Tpp, "9t")]
        [DataRow(ErrorCategory.PatientPracticeMessages, 502, SourceApi.Vision, "9s")]
        [DataRow(ErrorCategory.Timeout, 504, SourceApi.Emis, "ze")]
        [DataRow(ErrorCategory.Timeout, 504, SourceApi.Microtest, "zm")]
        [DataRow(ErrorCategory.Timeout, 504, SourceApi.Tpp, "zt")]
        [DataRow(ErrorCategory.Timeout, 504, SourceApi.Vision, "zs")]
        [DataRow(ErrorCategory.Timeout, 504, SourceApi.NhsLogin, "zn")]
        [DataRow(ErrorCategory.Timeout, 504, SourceApi.OrganDonation, "zo")]
        [DataRow(ErrorCategory.Timeout, 504, SourceApi.ServiceJourneyRules, "zj")]
        public void GenerateAndLogErrorReference_UsingErrorCategoryAndStatusCodeAndSourceApi_ReturnsCorrectCode(ErrorCategory errorCategory, int statusCode, SourceApi sourceApi, string expectedPrefix)
        {
            var errorCode = _errorReferenceGenerator.GenerateAndLogErrorReference(errorCategory, statusCode, sourceApi);

            errorCode.Should().StartWith(expectedPrefix);
        }

        [TestMethod]
        public void GenerateAndLogErrorReference__UsingErrorCategoryStatusCodeSourceApi_ReturnsCorrectCode()
        {
            var errorCode = _errorReferenceGenerator.GenerateAndLogErrorReference(ErrorCategory.None,999, SourceApi.None);

            errorCode.Should().StartWith("xx");
        }
        
        [TestMethod]
        public void GenerateAndLogErrorReference_UsingErrorCategoryAndStatusCodeThatReturnsMoreThanOneObject_ReturnsXxCode()
        {
            var errorCode = _errorReferenceGenerator.GenerateAndLogErrorReference(ErrorCategory.Login, 500, SourceApi.ServiceJourneyRules);

            errorCode.Should().StartWith("xx");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ErrorReferenceGeneratorTests()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _randomStringGenerator.Dispose();
            }

            _disposed = true;
        }
    }
}