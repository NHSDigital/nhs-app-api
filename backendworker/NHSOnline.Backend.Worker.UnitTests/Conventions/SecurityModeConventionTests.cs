using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Conventions;

namespace NHSOnline.Backend.Worker.UnitTests.Conventions
{
    [TestClass]
    public class SecurityModeConventionTests
    {
        private IFixture _fixture;
        private TypeInfo _controllerTypeInfo;
        private Mock<MethodInfo> _actionMethodInfo;
        private Mock<SelectorModel> _selectorModel;
        private Mock<ILogger<SecurityModeConvention>> _mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            _mockLogger = new Mock<ILogger<SecurityModeConvention>>();
            _controllerTypeInfo = typeof(Controller).GetTypeInfo();
            _actionMethodInfo = _fixture.Create<Mock<MethodInfo>>();
            _selectorModel = _fixture.Create<Mock<SelectorModel>>();
        }
        
        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow(null, RunMode.Cid)]
        [DataRow(null, RunMode.Pfs)]
        [DataRow(null, RunMode.All)]
        [DataRow(RunMode.Cid, null)]
        [DataRow(RunMode.Cid, RunMode.Cid)]
        [DataRow(RunMode.Cid, RunMode.Pfs)]
        [DataRow(RunMode.Cid, RunMode.All)]
        [DataRow(RunMode.Pfs, null)]
        [DataRow(RunMode.Pfs, RunMode.Cid)]
        [DataRow(RunMode.Pfs, RunMode.Pfs)]
        [DataRow(RunMode.Pfs, RunMode.All)]
        [DataRow(RunMode.All, null)]
        [DataRow(RunMode.All, RunMode.Cid)]
        [DataRow(RunMode.All, RunMode.Pfs)]
        [DataRow(RunMode.All, RunMode.All)]
        public void ConventionTest_ControllerSecurityAttribute_NoneRunMode(RunMode? actionRunMode, RunMode? controllerRunMode)
        {
            //Arrange
            var actionAttribute = GetSecurityModeAttribute(actionRunMode);
            var controllerAttribute = GetSecurityModeAttribute(controllerRunMode);
            var actionModel = ArrangeActionModel(actionAttribute, controllerAttribute);
            
            var systemUnderTest = new SecurityModeConvention(RunMode.None, _mockLogger.Object);

            actionModel.Selectors.Should().HaveCount(1);
            
            //Act
            systemUnderTest.Apply(actionModel);

            //Assert
            actionModel.Selectors.Should().HaveCount(0);
        }
        
        [DataTestMethod]
        [DataRow(null, RunMode.Cid)]
        [DataRow(null, RunMode.Pfs)]
        [DataRow(RunMode.Cid, null)]
        [DataRow(RunMode.Cid, RunMode.Cid)]
        [DataRow(RunMode.Cid, RunMode.Pfs)]
        [DataRow(RunMode.Pfs, null)]
        [DataRow(RunMode.Pfs, RunMode.Cid)]
        [DataRow(RunMode.Pfs, RunMode.Pfs)]
        [DataRow(RunMode.All, RunMode.Pfs)]
        [DataRow(RunMode.All, RunMode.Cid)]
        [DataRow(RunMode.Pfs, RunMode.All)]
        [DataRow(RunMode.Cid, RunMode.All)]
        [DataRow(RunMode.All, RunMode.All)]
        public void ConventionTest_ValidSecurityAttribute_DevRunMode(RunMode? actionRunMode, RunMode? controllerRunMode)
        {
            //Arrange
            var actionAttribute = GetSecurityModeAttribute(actionRunMode);
            var controllerAttribute = GetSecurityModeAttribute(controllerRunMode);
            var actionModel = ArrangeActionModel(actionAttribute, controllerAttribute);
            
            var systemUnderTest = new SecurityModeConvention(RunMode.Dev, _mockLogger.Object);

            actionModel.Selectors.Should().HaveCount(1);
            
            //Act
            systemUnderTest.Apply(actionModel);

            //Assert
            actionModel.Selectors.Should().HaveCount(1);
        }
        
        [DataTestMethod]
        [DataRow(null, null)]
        public void ConventionTest_InvalidSecurityAttribute_DevRunMode(RunMode? actionRunMode, RunMode? controllerRunMode)
        {
            //Arrange
            var actionAttribute = GetSecurityModeAttribute(actionRunMode);
            var controllerAttribute = GetSecurityModeAttribute(controllerRunMode);
            var actionModel = ArrangeActionModel(actionAttribute, controllerAttribute);
            
            var systemUnderTest = new SecurityModeConvention(RunMode.Dev, _mockLogger.Object);

            actionModel.Selectors.Should().HaveCount(1);
            
            //Act
            systemUnderTest.Apply(actionModel);

            //Assert
            actionModel.Selectors.Should().HaveCount(0);
        }
        
        [DataTestMethod]
        [DataRow(null, RunMode.Cid)]
        [DataRow(null, RunMode.All)]
        [DataRow(RunMode.Cid, null)]
        [DataRow(RunMode.All, null)]
        [DataRow(RunMode.Cid, RunMode.Cid)]
        [DataRow(RunMode.Cid, RunMode.Pfs)]
        [DataRow(RunMode.Cid, RunMode.All)]
        [DataRow(RunMode.All, RunMode.Cid)]
        [DataRow(RunMode.All, RunMode.Pfs)]
        [DataRow(RunMode.All, RunMode.All)]
        public void ConventionTest_ValidSecurityAttribute_CidRunMode(RunMode? actionRunMode, RunMode? controllerRunMode)
        {
            //Arrange
            var actionAttribute = GetSecurityModeAttribute(actionRunMode);
            var controllerAttribute = GetSecurityModeAttribute(controllerRunMode);
            var actionModel = ArrangeActionModel(actionAttribute, controllerAttribute);
            
            var systemUnderTest = new SecurityModeConvention(RunMode.Cid, _mockLogger.Object);

            actionModel.Selectors.Should().HaveCount(1);
            
            //Act
            systemUnderTest.Apply(actionModel);

            //Assert
            actionModel.Selectors.Should().HaveCount(1);
        }
        
        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow(null, RunMode.Pfs)]
        [DataRow(RunMode.Pfs, null)]
        [DataRow(RunMode.Pfs, RunMode.Cid)]
        [DataRow(RunMode.Pfs, RunMode.Pfs)]
        [DataRow(RunMode.Pfs, RunMode.All)]
        public void ConventionTest_InvalidSecurityAttribute_CidRunMode(RunMode? actionRunMode, RunMode? controllerRunMode)
        {
            //Arrange
            var actionAttribute = GetSecurityModeAttribute(actionRunMode);
            var controllerAttribute = GetSecurityModeAttribute(controllerRunMode);
            var actionModel = ArrangeActionModel(actionAttribute, controllerAttribute);
            
            var systemUnderTest = new SecurityModeConvention(RunMode.Cid, _mockLogger.Object);

            actionModel.Selectors.Should().HaveCount(1);
            
            //Act
            systemUnderTest.Apply(actionModel);

            //Assert
            actionModel.Selectors.Should().HaveCount(0);
        }
        
        [DataTestMethod]
        [DataRow(null, RunMode.Pfs)]
        [DataRow(null, RunMode.All)]
        [DataRow(RunMode.Pfs, null)]
        [DataRow(RunMode.All, null)]
        [DataRow(RunMode.Pfs, RunMode.Cid)]
        [DataRow(RunMode.Pfs, RunMode.Pfs)]
        [DataRow(RunMode.Pfs, RunMode.All)]
        [DataRow(RunMode.All, RunMode.Cid)]
        [DataRow(RunMode.All, RunMode.Pfs)]
        [DataRow(RunMode.All, RunMode.All)]
        public void ConventionTest_ValidSecurityAttribute_PfsRunMode(RunMode? actionRunMode, RunMode? controllerRunMode)
        {
            //Arrange
            var actionAttribute = GetSecurityModeAttribute(actionRunMode);
            var controllerAttribute = GetSecurityModeAttribute(controllerRunMode);
            var actionModel = ArrangeActionModel(actionAttribute, controllerAttribute);
            
            var systemUnderTest = new SecurityModeConvention(RunMode.Pfs, _mockLogger.Object);

            actionModel.Selectors.Should().HaveCount(1);
            
            //Act
            systemUnderTest.Apply(actionModel);

            //Assert
            actionModel.Selectors.Should().HaveCount(1);
        }
        
        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow(null, RunMode.Cid)]
        [DataRow(RunMode.Cid, null)]
        [DataRow(RunMode.Cid, RunMode.Cid)]
        [DataRow(RunMode.Cid, RunMode.Pfs)]
        [DataRow(RunMode.Cid, RunMode.All)]
        public void ConventionTest_InvalidSecurityAttribute_PfsRunMode(RunMode? actionRunMode, RunMode? controllerRunMode)
        {
            //Arrange
            var actionAttribute = GetSecurityModeAttribute(actionRunMode);
            var controllerAttribute = GetSecurityModeAttribute(controllerRunMode);
            var actionModel = ArrangeActionModel(actionAttribute, controllerAttribute);
            
            var systemUnderTest = new SecurityModeConvention(RunMode.Pfs, _mockLogger.Object);

            actionModel.Selectors.Should().HaveCount(1);
            
            //Act
            systemUnderTest.Apply(actionModel);

            //Assert
            actionModel.Selectors.Should().HaveCount(0);
        }

        private static SecurityModeAttribute GetSecurityModeAttribute(RunMode? runMode)
        {
            switch (runMode)
            {
                case RunMode.Cid:
                    return new CidSecurityModeAttribute();
                case RunMode.Pfs:
                    return new PfsSecurityModeAttribute();
                case RunMode.All:
                    return new AllSecurityModeAttribute();
                default:
                    return null;
            }
        }

        private ControllerModel ArrangeControllerModel(Attribute controllerAttribute)
        {
            var controllerAttributeList = new List<Attribute>();
            
            if(controllerAttribute != null)
                controllerAttributeList.Add(controllerAttribute);
            
            var controllerModel = new ControllerModel(_controllerTypeInfo,
                controllerAttributeList);

            return controllerModel;
        }
        
        private ActionModel ArrangeActionModel(Attribute actionAttribute, Attribute controllerAttribute)
        {
            var actionAttributeList = new List<Attribute>();
            if(actionAttribute != null)
                actionAttributeList.Add(actionAttribute);
            
            var actionModel = new ActionModel(_actionMethodInfo.Object,
                actionAttributeList);
            actionModel.Controller = ArrangeControllerModel(controllerAttribute);

            actionModel.Selectors.Add(_selectorModel.Object);

            return actionModel;
        }
    }
}
