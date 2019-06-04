using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.Support.UnitTests
{
    [TestClass]
    public class EnumHelperTests
    {
        [TestMethod]
        public void GetType_WhenTypeNotEnum_ThrownsAnException()
        {
            // act
            Action act = () => EnumHelper.GetType<int>();

            // assert
            act.Should().Throw<ArgumentException>().Which.Message
                .Should().Be("Type provided must be an Enum.");
        }

        [TestMethod]
        public void GetType_WhenTypeIsEnum_ReturnsType()
        {
            // act
            var result = EnumHelper.GetType<Test>();

            // assert
            result.Should().Be(typeof(Test));
        }

        public enum Test
        {
            First = 0,

            [System.ComponentModel.Description("Description 1")]
            Second,

            [System.ComponentModel.Description("Description 2")]
            Third,

            Fourth
        }

        [TestMethod]
        public void GetDescriptionOrThrowException_WhenNoDescrption_ThrownsAnException()
        {
            // act
            Action act = () => EnumHelper.GetDescriptionOrThrowException(Test.Fourth);

            // assert
            act.Should().Throw<ArgumentException>().Which.Message
                .Should().Be("No Description attribute on enum Fourth");
        }

        [TestMethod]
        public void GetDescriptionOrThrowException_WhenDescrption_ReturnsDescription()
        {
            // act
            var result = EnumHelper.GetDescriptionOrThrowException(Test.Third);

            // assert
            result.Should().Be("Description 2");
        }
    }
}