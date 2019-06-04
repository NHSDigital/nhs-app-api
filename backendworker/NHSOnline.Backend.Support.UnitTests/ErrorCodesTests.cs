using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.Support.UnitTests
{
    [TestClass]
    public class ErrorCodesTests
    {
        [TestMethod]
        public void Success()
        {
            var errorResponses = new ErrorCodes<SuccessTest>().GetAllErrorResponses();

            errorResponses.Should().HaveCount(2);

            errorResponses[1].ErrorCode.Should().Be(1);
            errorResponses[1].ErrorMessage.Should().Be("First Value");
            errorResponses[2].ErrorCode.Should().Be(2);
            errorResponses[2].ErrorMessage.Should().Be("Second Value");
        }

        private enum SuccessTest
        {
            [System.ComponentModel.Description("First Value")]
            ValueOne = 1,

            [System.ComponentModel.Description("Second Value")]
            ValueTwo = 2,
        }

        [TestMethod]
        public void DuplicateValueFailure()
        {
            var errorCodes = new ErrorCodes<DuplicateValueTest>();

            Action act = () => errorCodes.GetAllErrorResponses();

            act.Should().Throw<ArgumentException>().And.Message.Should()
                .Be("An item with the same key has already been added. Key: 1");
        }

        private enum DuplicateValueTest
        {
            [System.ComponentModel.Description("First Value")]
            ValueOne = 1,
            [System.ComponentModel.Description("Second Value")]
            ValueTwo = 2,
            [System.ComponentModel.Description("Third Value")]
            ValueThree = 1,
        }

        [TestMethod]
        public void NotAnEnumFailure()
        {
            var errorCodes = new ErrorCodes<int>();

            Action act = () => errorCodes.GetAllErrorResponses();

            act.Should().Throw<ArgumentException>().And.Message.Should()
                .Contain("Type provided must be an Enum");
        }

        [TestMethod]
        public void NoDescriptionFailure()
        {
            var errorCodes = new ErrorCodes<NoDescriptionTest>();

            Action act = () => errorCodes.GetAllErrorResponses();

            act.Should().Throw<ArgumentException>().And.Message.Should()
                .Be("No Description attribute on enum ValueThree");
        }
        
        private enum NoDescriptionTest
        {
            [System.ComponentModel.Description("First Value")]
            ValueOne = 1,
            [System.ComponentModel.Description("Second Value")]
            ValueTwo = 2,
            ValueThree = 3,
        }
    }
}