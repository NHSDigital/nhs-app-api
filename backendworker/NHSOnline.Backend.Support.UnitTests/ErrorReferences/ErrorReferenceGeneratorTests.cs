using System;
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
        [DataRow(typeof(ErrorTypes.LoginServiceJourneyRulesOdsCodeNotFound), "3j")]
        [DataRow(typeof(ErrorTypes.LoginServiceJourneyRulesOtherError), "3k")]
        public void Generate_ErrorReferenceCode_Login_UsingErrorType_ReturnsCorrectCode(Type type, string expectedPrefix)
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
        public void Generate_ErrorReferenceCode_Login_UsingErrorCategoryAndStatusCode_ReturnsCorrectCode(ErrorCategory errorCategory, int statusCode, string expectedPrefix)
        {
            var errorCode = _errorReferenceGenerator.GenerateAndLogErrorReference(errorCategory, statusCode);

            errorCode.Should().StartWith(expectedPrefix);
        }
        
        [DataTestMethod]
        [DataRow(ErrorCategory.Login, 502, Supplier.Emis, "3e")]
        [DataRow(ErrorCategory.Login, 502, Supplier.Microtest, "3m")]
        [DataRow(ErrorCategory.Login, 502, Supplier.Tpp, "3t")]
        [DataRow(ErrorCategory.Login, 502, Supplier.Vision, "3s")]
        public void Generate_ErrorReferenceCode_Login_UsingErrorCategoryAndStatusCodeAndSupplier_ReturnsCorrectCode(ErrorCategory errorCategory, int statusCode, Supplier supplier, string expectedCode)
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
        public void Generate_ErrorReferenceCode_Login_UsingErrorCategoryAndStatusCodeAndSourceApi_ReturnsCorrectCode(ErrorCategory errorCategory, int statusCode, SourceApi sourceApi, string expectedPrefix)
        {
            var errorCode = _errorReferenceGenerator.GenerateAndLogErrorReference(errorCategory, statusCode, sourceApi);

            errorCode.Should().StartWith(expectedPrefix);
        }
        
        [DataTestMethod]
        [DataRow(ErrorCategory.Timeout, 504, SourceApi.Emis, "ze")]
        [DataRow(ErrorCategory.Timeout, 504, SourceApi.Microtest, "zm")]
        [DataRow(ErrorCategory.Timeout, 504, SourceApi.Tpp, "zt")]
        [DataRow(ErrorCategory.Timeout, 504, SourceApi.Vision, "zs")]
        [DataRow(ErrorCategory.Timeout, 504, SourceApi.NhsLogin, "zn")]
        [DataRow(ErrorCategory.Timeout, 504, SourceApi.OrganDonation, "zo")]
        [DataRow(ErrorCategory.Timeout, 504, SourceApi.ServiceJourneyRules, "zj")]
        public void Generate_ErrorReferenceCode_Timeout_UsingErrorCategoryAndStatusCodeAndSourceApi_ReturnsCorrectCode(ErrorCategory errorCategory, int statusCode, SourceApi sourceApi, string expectedPrefix)
        {
            var errorCode = _errorReferenceGenerator.GenerateAndLogErrorReference(errorCategory, statusCode, sourceApi);

            errorCode.Should().StartWith(expectedPrefix);
        }
        
        [DataTestMethod]
        [DataRow(ErrorCategory.Timeout, 504, Supplier.Emis, "ze")]
        [DataRow(ErrorCategory.Timeout, 504, Supplier.Microtest, "zm")]
        [DataRow(ErrorCategory.Timeout, 504, Supplier.Tpp, "zt")]
        [DataRow(ErrorCategory.Timeout, 504, Supplier.Vision, "zs")]
        public void Generate_ErrorReferenceCode_Timeout_UsingErrorCategoryAndStatusCodeAndSupplier_ReturnsCorrectCode(ErrorCategory errorCategory, int statusCode, Supplier supplier, string expectedPrefix)
        {
            var errorCode = _errorReferenceGenerator.GenerateAndLogErrorReference(errorCategory, statusCode, supplier);

            errorCode.Should().StartWith(expectedPrefix);
        }

        [TestMethod]
        public void Generate_ErrorReferenceCodeForInvalidData_Login_UsingErrorCategoryStatusCodeSourceApi_ReturnsCorrectCode()
        {
            var errorCode = _errorReferenceGenerator.GenerateAndLogErrorReference(ErrorCategory.None,999, SourceApi.None);

            errorCode.Should().StartWith("xx");
        }
        
        [TestMethod]
        public void Generate_ErrorReferenceCode_Login_UsingErrorCategoryAndStatusCodeThatReturnsMoreThanOneObject_ReturnsXxCode()
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