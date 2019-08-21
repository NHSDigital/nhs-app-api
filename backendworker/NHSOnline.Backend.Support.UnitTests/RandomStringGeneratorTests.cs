using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.Support.UnitTests
{
    [TestClass]
    public class RandomStringGeneratorTests
    {
        private IRandomStringGenerator _randomStringGenerator;
        private const string CharactersForGenerator = "acefghjkmnorstuwxyz3456789";
        private const string ExpectedString = "aszk";
        
        private class DeterministicRandomGenerator : System.Security.Cryptography.RandomNumberGenerator
        {
            Random r = new Random(0);
            public override void GetBytes(byte[] data)
            {
                r.NextBytes(data);
            }
            public override void GetNonZeroBytes(byte[] data)
            {
                for (var i = 0; i < data.Length; i++)
                    data[i] = (byte)r.Next(1, 256);
            }
        }
        
        [TestInitialize]
        public void TestInitialize()
        {
            _randomStringGenerator = new RandomStringGenerator(new DeterministicRandomGenerator());
        }
        
        [DataTestMethod]
        [DataRow(4)]
        [DataRow(9)]
        public void GenerateString_StringLengthSpecified_GeneratedStringIsOfSpecifiedLength(int stringSize)
        {
            var randomString1 = _randomStringGenerator.GenerateString(stringSize, CharactersForGenerator);

            randomString1.Length.Should().Be(stringSize);
        }
        
        [TestMethod]
        public void GenerateString_AllowableCharactersSpecified_GeneratedStringOnlyContainsSpecifiedCharacters()
        {
            var randomString = _randomStringGenerator.GenerateString(4, CharactersForGenerator);

            Assert.IsFalse(Regex.IsMatch(randomString,$"[^{CharactersForGenerator}]"));
        }

        [TestMethod]
        public void GenerateString_InvokedMultipleTimes_EachStringIsDifferent()
        {
            var randomString1 = _randomStringGenerator.GenerateString(4, CharactersForGenerator);
            var randomString2 = _randomStringGenerator.GenerateString(4, CharactersForGenerator);

            randomString1.Should().NotMatch(randomString2);
        }

        [TestMethod]
        public void GenerateString_Invoked_FirstTimeStringEqualsExpectedValue()
        {
            
            var randomString1 = _randomStringGenerator.GenerateString(4, CharactersForGenerator);
           
            randomString1.Should().Match(ExpectedString);
        }
    }
}