using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.Support.UnitTests
{
    [TestClass]
    public class EnumHelperTests
    {

        [TestMethod]
        public void TryParseFromDescription_WhenTypeNotEnum_ThrownsAnException()
        {
            // act
            Action act = () => EnumHelper.TryParseFromDescription<int>("Description 1", out _);
            
            // assert
            act.Should().Throw<ArgumentException>().Which.Message
                .Should().Be("Type provided must be an Enum.");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("      ")]
        [DataRow("Description Invalid")]
        public void TryParseFromDescription_WhenGivenInvalidDescription_FailsToParse(string description)
        {
            // act
            var result = EnumHelper.TryParseFromDescription<Test>(description, out var value);
            
            // assert
            result.Should().BeFalse();
            value.Should().Be(Test.First);
        }

        [TestMethod]
        [DataRow("Second")]
        [DataRow("Third")]
        public void TryParseFromDescription_WhenGivenEnumNameThatHasDescription_FailsToParse(string name)
        {
            // act
            var result = EnumHelper.TryParseFromDescription<Test>(name, out var value);
            
            // assert
            result.Should().BeFalse();
            value.Should().Be(Test.First);
        }

        [TestMethod]
        [DataRow("First", Test.First)]
        [DataRow("Fourth", Test.Fourth)]
        public void TryParseFromDescription_WhenGivenEnumNameWithNoDescription_MatchesByName(string name, Test expectedValue)
        {
            // act
            var result = EnumHelper.TryParseFromDescription<Test>(name, out var value);
            
            // assert
            result.Should().BeTrue();
            value.Should().Be(expectedValue);
        }
        
        [TestMethod]
        [DataRow("Description 1", Test.Second)]
        [DataRow("Description 2", Test.Third)]
        public void TryParseFromDescription_WhenGivenValidDescriptions_MatchesByDescription(string description, Test expectedValue)
        {
            // act
            var result = EnumHelper.TryParseFromDescription<Test>(description, out var value);
            
            // assert
            result.Should().BeTrue();
            value.Should().Be(expectedValue);
        }

        [TestMethod]
        public void ParseFromDescription_WhenTypeNotEnum_ThrownsAnException()
        {
            // act
            Action act = () => EnumHelper.ParseFromDescription<int>("Description 1");
            
            // assert
            act.Should().Throw<ArgumentException>().Which.Message
                .Should().Be("Type provided must be an Enum.");
        }

        [TestMethod]
        public void ParseFromDescription_WhenDescriptionIsNull_ThrownsAnException()
        {
            // act
            Action act = () => EnumHelper.ParseFromDescription<Test>(null);
            
            // assert
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("description");
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("      ")]
        public void ParseFromDescription_WhenDescriptionIsEmptyOrWhiteSpace_ThrownsAnException(string description)
        {
            // act
            Action act = () => EnumHelper.ParseFromDescription<Test>(description);
            
            // assert
            act.Should().Throw<ArgumentException>().Which.Message
                .Should().Be("Must specify valid information for parsing in the string.");
        }

        [TestMethod]
        public void ParseFromDescription_WhenGivenInvalidDescription_ThrownsAnException()
        {
            // arrange
            var description = "Invalid description";
            
            // act
            Action act = () => EnumHelper.ParseFromDescription<Test>(description);
            
            // assert
            act.Should().Throw<ArgumentException>().Which.Message
                .Should().Be($"Requested value '{description}' was not found.");
        }

        [TestMethod]
        [DataRow("Second")]
        [DataRow("Third")]
        public void ParseFromDescription_WhenGivenEnumNameThatHasDescription_ThrownsAnException(string name)
        {
            // act
            Action act = () => EnumHelper.ParseFromDescription<Test>(name);
            
            // assert
            act.Should().Throw<ArgumentException>().Which.Message
                .Should().Be($"Requested value '{name}' was not found.");
        }

        [TestMethod]
        [DataRow("First", Test.First)]
        [DataRow("Fourth", Test.Fourth)]
        public void ParseFromDescription_WhenGivenEnumNameWithNoDescription_MatchesByName(string name, Test expectedValue)
        {
            // act
            var result = EnumHelper.ParseFromDescription<Test>(name);
            
            // assert
            result.Should().Be(expectedValue);
        }
        
        [TestMethod]
        [DataRow("Description 1", Test.Second)]
        [DataRow("Description 2", Test.Third)]
        public void ParseFromDescription_WhenGivenValidDescriptions_MatchesByDescription(string description, Test expectedValue)
        {
            // act
            var result = EnumHelper.ParseFromDescription<Test>(description);
            
            // assert
            result.Should().Be(expectedValue);
        }

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
        
    }
}