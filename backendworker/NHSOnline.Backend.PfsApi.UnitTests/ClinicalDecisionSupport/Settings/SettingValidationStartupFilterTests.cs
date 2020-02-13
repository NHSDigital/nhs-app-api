using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet.Filters;

namespace NHSOnline.Backend.PfsApi.UnitTests.ClinicalDecisionSupport.Settings
{
    [TestClass]
    public class SettingValidationStartupFilterTests
    {
        private Mock<Action<IApplicationBuilder>> _mockAction;
        private Mock<IValidatable> _mockValidatableObjectOne;
        private Mock<IValidatable> _mockValidatableObjectTwo;
        private Mock<List<IValidatable>> _mockValidatableObjects;
        private SettingValidationStartupFilter _startupFilter;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAction = new Mock<Action<IApplicationBuilder>>();
            
            _mockValidatableObjects = new Mock<List<IValidatable>>();
            
            _mockValidatableObjectOne = new Mock<IValidatable>();
            _mockValidatableObjectTwo = new Mock<IValidatable>();
            
            _mockValidatableObjects.Object.Add(_mockValidatableObjectOne.Object);
            _mockValidatableObjects.Object.Add(_mockValidatableObjectTwo.Object);
            
            _startupFilter = new SettingValidationStartupFilter(_mockValidatableObjects.Object);
        }

        [TestMethod]
        public void Configure_InvokesValidateOnAllValidatableObjects()
        {
            // Arrange
            _mockValidatableObjectOne.Setup(v => v.Validate());
            _mockValidatableObjectTwo.Setup(v => v.Validate());
            
            // Act
            var next = _startupFilter.Configure(_mockAction.Object);
            
            // Assert
            _mockValidatableObjectOne.Verify(v => v.Validate(), Times.Once);
            _mockValidatableObjectTwo.Verify(v => v.Validate(), Times.Once);
            next.Should().BeEquivalentTo(_mockAction.Object);
        }
    }
}