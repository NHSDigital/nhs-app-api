using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Hl7.Fhir.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;

namespace NHSOnline.Backend.PfsApi.UnitTests.ClinicalDecisionSupport.ServiceDefinition
{
    [TestClass]
    public class ServiceDefinitionListBuilderTests
    {
        [TestMethod]
        public void ServiceDefinitionListBuilder_BuildServiceDefinitionList_Builds_Empty_List_When_Bundle_IsEmpty()
        {
            // Arrange
            var sut = new ServiceDefinitionListBuilder();
            var bundle = new Bundle();

            // Act
            var list = sut.BuildServiceDefinitionList(bundle);

            // Assert
            list.Should().BeEmpty();
        }

        [TestMethod]
        public void
            ServiceDefinitionListBuilder_BuildServiceDefinitionList_Builds_Correct_List_When_Bundle_Has_One_Service_Definitions_With_No_Aliases()
        {
            // Arrange
            var sut = new ServiceDefinitionListBuilder();
            var bundle = new Bundle();

            var sd1 = new Hl7.Fhir.Model.ServiceDefinition
            {
                Id = "ID1",
                Title = "Title 1",
                Topic = new List<CodeableConcept>
                {
                    new CodeableConcept("system", "WHL", "Category 1", "Category 1 Text")
                },
            };

            bundle.AddResourceEntry(sd1, "http://ems.cdss.stubs.local.bitraft.io:8088/fhir/ServiceDefinition/ID1");

            // Act
            var list = sut.BuildServiceDefinitionList(bundle);

            // Assert
            var expected = new List<ServiceDefinitionCategory>()
            {
                new ServiceDefinitionCategory("Category 1")
                {
                    Items = { new ServiceDefinitionItem("ID1", "Title 1") }
                }
            };

            list.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void
            ServiceDefinitionListBuilder_BuildServiceDefinitionList_Builds_Correct_List_When_Bundle_Has_One_Service_Definitions_With_One_Alias()
        {
            // Arrange
            var sut = new ServiceDefinitionListBuilder();
            var bundle = new Bundle();

            var useContext1 = new UsageContext
            {
                Code = new Coding
                {
                    Display = "Alias 1"
                }
            };

            var sd1 = new Hl7.Fhir.Model.ServiceDefinition
            {
                Id = "ID1",
                Title = "Title 1",
                Topic = new List<CodeableConcept>
                {
                    new CodeableConcept("system", "WHL", "Category 1", "Category 1 Text")
                },
                UseContext = new List<UsageContext>
                {
                    useContext1
                }
            };

            bundle.AddResourceEntry(sd1, "http://ems.cdss.stubs.local.bitraft.io:8088/fhir/ServiceDefinition/ID1");

            // Act
            var list = sut.BuildServiceDefinitionList(bundle);

            // Assert
            var expected = new List<ServiceDefinitionCategory>()
            {
                new ServiceDefinitionCategory("Category 1")
                {
                    Items =
                    {
                        new ServiceDefinitionItem("ID1", "Alias 1"),
                        new ServiceDefinitionItem("ID1", "Title 1")
                    }
                }
            };

            list.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void
            ServiceDefinitionListBuilder_BuildServiceDefinitionList_Builds_Must_Combine_ServiceDefinitions_Into_Categories()
        {
            // Arrange
            var sut = new ServiceDefinitionListBuilder();
            var bundle = new Bundle();

            var sd1 = new Hl7.Fhir.Model.ServiceDefinition
            {
                Id = "ID1",
                Title = "Title 1",
                Topic = new List<CodeableConcept>
                {
                    new CodeableConcept("system", "WHL", "Category 1", "Category 1 Text")
                },
                UseContext = new List<UsageContext>
                {
                    new UsageContext
                    {
                        Code = new Coding
                        {
                            Display = "Alias 1"
                        }
                    }
                }
            };

            var sd2 = new Hl7.Fhir.Model.ServiceDefinition
            {
                Id = "ID2",
                Title = "Title 2",
                Topic = new List<CodeableConcept>
                {
                    new CodeableConcept("system", "WHL", "Category 1", "Category 1 Text")
                },
                UseContext = new List<UsageContext>
                {
                    new UsageContext
                    {
                        Code = new Coding
                        {
                            Display = "Alias 2"
                        }
                    }
                }
            };

            bundle.AddResourceEntry(sd1, "http://ems.cdss.stubs.local.bitraft.io:8088/fhir/ServiceDefinition/ID1");
            bundle.AddResourceEntry(sd2, "http://ems.cdss.stubs.local.bitraft.io:8088/fhir/ServiceDefinition/ID2");

            // Act
            var list = sut.BuildServiceDefinitionList(bundle);

            // Assert
            var expected = new List<ServiceDefinitionCategory>()
            {
                new ServiceDefinitionCategory("Category 1")
                {
                    Items =
                    {
                        new ServiceDefinitionItem("ID1", "Alias 1"),
                        new ServiceDefinitionItem("ID2", "Alias 2"),
                        new ServiceDefinitionItem("ID1", "Title 1"),
                        new ServiceDefinitionItem("ID2", "Title 2")
                    }
                }
            };

            list.Should().BeEquivalentTo(expected);
        }
    }
}