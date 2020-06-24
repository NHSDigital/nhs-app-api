using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.Support.UnitTests
{
    [TestClass]
    public class CitizenIdUserSessionTests
    {
        [DataTestMethod]
        [DataRow("Aneurin", "Bevan", "Aneurin Bevan")]
        [DataRow("Florence", "Nightingale", "Florence Nightingale")]
        public void Name_IsBuiltFromConcatenatedGivenNameAndFamilyName(string givenName, string familyName,
            string expectedName)
        {
            // Arrange
            var systemUnderTest = new CitizenIdUserSession()
            {
                GivenName = givenName,
                FamilyName = familyName
            };

            // Act
            var actualName = systemUnderTest.Name;

            // Assert
            actualName.Should().Be(expectedName);
        }
    }
}