using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition;

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
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void ServiceDefinitionListBuilder_BuildServiceDefinitionList_Builds_Correct_List_When_Bundle_Has_One_Service_Definitions_With_No_Aliases()
        {
            // Arrange
            var sut = new ServiceDefinitionListBuilder();
            var bundle = new Bundle();

            var sd1 = new Hl7.Fhir.Model.ServiceDefinition {
                Id = "ID1",
                Title = "Title 1",
                Topic = new List<CodeableConcept> {
                    new CodeableConcept("system", "WHL", "Category 1", "Category 1 Text")
                },
            };

            bundle.AddResourceEntry(sd1, "http://ems.cdss.stubs.local.bitraft.io:8088/fhir/ServiceDefinition/ID1");

            // Act
            var list = sut.BuildServiceDefinitionList(bundle);

            // Assert
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Category 1", list.First().Category);

            Assert.AreEqual(1, list.First().Items.Count);

            Assert.AreEqual("ID1", list.First().Items[0].Id);
            Assert.AreEqual("Title 1", list.First().Items[0].Title);
        }

         [TestMethod]
        public void ServiceDefinitionListBuilder_BuildServiceDefinitionList_Builds_Correct_List_When_Bundle_Has_One_Service_Definitions_With_One_Alias()
        {
            // Arrange
            var sut = new ServiceDefinitionListBuilder();
            var bundle = new Bundle();

            var useContext1 = new UsageContext {
                Code = new Coding {
                    Display = "Alias 1"
                }
            };

            var sd1 = new Hl7.Fhir.Model.ServiceDefinition {
                Id = "ID1",
                Title = "Title 1",
                Topic = new List<CodeableConcept> {
                    new CodeableConcept("system", "WHL", "Category 1", "Category 1 Text")
                },
                UseContext = new List<UsageContext> {
                    useContext1
                }
            };

            bundle.AddResourceEntry(sd1, "http://ems.cdss.stubs.local.bitraft.io:8088/fhir/ServiceDefinition/ID1");

            // Act
            var list = sut.BuildServiceDefinitionList(bundle);

            // Assert
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Category 1", list.First().Category);

            Assert.AreEqual(2, list.First().Items.Count);

            Assert.AreEqual("ID1", list.First().Items[0].Id);
            Assert.AreEqual("Alias 1", list.First().Items[0].Title);

            Assert.AreEqual("ID1", list.First().Items[1].Id);
            Assert.AreEqual("Title 1", list.First().Items[1].Title);
        }

        [TestMethod]
        public void ServiceDefinitionListBuilder_BuildServiceDefinitionList_Builds_Must_Combine_ServiceDefinitions_Into_Categories()
        {
            // Arrange
            var sut = new ServiceDefinitionListBuilder();
            var bundle = new Bundle();

            var sd1 = new Hl7.Fhir.Model.ServiceDefinition {
                Id = "ID1",
                Title = "Title 1",
                Topic = new List<CodeableConcept> {
                    new CodeableConcept("system", "WHL", "Category 1", "Category 1 Text")
                },
                UseContext = new List<UsageContext> {
                    new UsageContext {
                        Code = new Coding {
                            Display = "Alias 1"
                        }
                    }
                }
            };

            var sd2 = new Hl7.Fhir.Model.ServiceDefinition {
                Id = "ID2",
                Title = "Title 2",
                Topic = new List<CodeableConcept> {
                    new CodeableConcept("system", "WHL", "Category 1", "Category 1 Text")
                },
                UseContext = new List<UsageContext> {
                    new UsageContext {
                        Code = new Coding {
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
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Category 1", list.First().Category);

            Assert.AreEqual(4, list.First().Items.Count);

            Assert.AreEqual("ID1", list.First().Items[0].Id);
            Assert.AreEqual("Alias 1", list.First().Items[0].Title);

            Assert.AreEqual("ID2", list.First().Items[1].Id);
            Assert.AreEqual("Alias 2", list.First().Items[1].Title);

             Assert.AreEqual("ID1", list.First().Items[2].Id);
            Assert.AreEqual("Title 1", list.First().Items[2].Title);

            Assert.AreEqual("ID2", list.First().Items[3].Id);
            Assert.AreEqual("Title 2", list.First().Items[3].Title);
        }
    }
}