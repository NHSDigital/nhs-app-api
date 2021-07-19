using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.OnlineConsultations.Models;

namespace NHSOnline.HttpMocks.OnlineConsultations
{
    [Route("fhir/ServiceDefinition")]
    public class OnlineConsultationsController : Controller
    {
        [HttpPost("$isValid")]
        public IActionResult IsValid()
        {
            return Json(new
            {
                resourceType = "Parameters",
                parameter = new[]
                {
                    new
                    {
                        name = "return",
                        valueBoolean = "true"
                    }
                }
            });
        }

        [HttpPost("{_}/$evaluate")]
        public IActionResult Evaluate([FromBody] OlcFhirRequest? content, string _)
        {
            var inputData = content?.Parameter?.FirstOrDefault(
                x => x.Name!.Equals("inputData", System.StringComparison.Ordinal)
            );

            if (inputData?.Resource?.ResourceType?.Equals("QuestionnaireResponse", System.StringComparison.Ordinal) != null)
            {
                return ConditionsResponse();
            }

            return TermsResponse();
        }

        private IActionResult TermsResponse()
        {
            return Json(new
            {
                resourceType = "GuidanceResponse",
                contained = new object[]
                {
                    new
                    {
                        resourceType ="Parameters",
                        id = "outputParams",
                    },
                    new {
                        resourceType ="Questionnaire",
                        id = "GLO_PRE_DISCLAIMERS_NHS_2",
                        status = "active",
                        item = new[]
                        {
                            new {
                                linkId = "BRP",
                                text = "<p>We're about to ask you a few questions about your request. " +
                                       "Your answers will be sent securely to your GP surgery unless urgent medical attention is needed. " +
                                       "For such cases the online consultation service will direct you to other health services.</p><br/>" +
                                       "<p>To start, please agree to the privacy notice applicable to online consultation services.</p>",
                                type = "group",
                                required = true,
                                item = new[]
                                {
                                    new {
                                        linkId = "BRP_BRP",
                                        text = "I have read the GP online consultation services privacy notice. " +
                                        "I understand the online consultation service provider will process my personal and health data " +
                                        "on behalf of my GP surgery to provide an online consultation.",
                                        type = "boolean",
                                        required = true
                                    }
                                }
                            }
                        }
                    }
                },
                module = new {
                    reference = "https://stubs.onlineconsultations/fhir/ServiceDefinition/BRP_BRP"
                },
                status = "data-required",
                occurrenceDateTime = "2020-02-21T17:04:45.098",
                outputParameters = new {
                    reference = "#outputParams"
                },
                dataRequirement = new object[]
                {
                    new
                    {
                        id = "PATIENT",
                        type = "Patient",
                        profile = new [] {
                            "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Patient-1"
                        }
                    },
                    new
                    {
                        id = "ORG",
                        type = "Organization",
                        profile = new [] {
                            "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Organization-1"
                        },
                        codeFilter = new []
                        {
                           new
                           {
                               path = "identifier",
                               valueSetString = "odsOrganizationCode"
                           }
                        }
                    },
                    new
                    {
                        id = "GLO_PRE_DISCLAIMERS_NHS_2",
                        extension = new []
                        {
                            new
                            {
                                url = "https://www.hl7.org/fhir/questionnaire.html",
                                valueReference = new {
                                   reference = "#GLO_PRE_DISCLAIMERS_NHS_2"
                                }
                            }
                        },
                        type = "QuestionnaireResponse",
                        profile = new [] {
                            "https://www.hl7.org/fhir/questionnaireresponse.html"
                        }
                    }
                }
            });
        }

        private IActionResult ConditionsResponse()
        {
            return Json(new
            {
                resourceType = "GuidanceResponse",
                contained = new[]
                {
                    new {
                        resourceType ="Questionnaire",
                        id = "CONDITION_LIST",
                        status = "active",
                        item = new[]
                        {
                            new {
                                linkId = "BRP",
                                text = "Breathing problems",
                                type = "group",
                                required = false,
                                item = new[]
                                {
                                    new {
                                        linkId = "BRP_BRP",
                                        text = "Breathing problems",
                                        type = "boolean",
                                        required = false
                                    }
                                }
                            },
                            new {
                                linkId = "EYE",
                                text = "Eye problems",
                                type = "group",
                                required = false,
                                item = new[]
                                {
                                    new {
                                        linkId = "EYE_CNJ",
                                        text = "Conjunctivitis",
                                        type = "boolean",
                                        required = false
                                    }
                                }
                            },
                            new {
                                linkId = "GBS",
                                text = "Gut, bowel & stomach",
                                type = "group",
                                required = false,
                                item = new[]
                                {
                                    new {
                                        linkId = "GBS_DAV",
                                        text = "Gastroenteritis",
                                        type = "boolean",
                                        required = false
                                    }
                                }
                            },
                            new {
                                linkId = "JTP",
                                text = "Joint pain",
                                type = "group",
                                required = false,
                                item = new[]
                                {
                                    new {
                                        linkId = "JTP_SPN",
                                        text = "Shoulder pain",
                                        type = "boolean",
                                        required = false
                                    }
                                }
                            },
                            new {
                                linkId = "LTC",
                                text = "Long term conditions",
                                type = "group",
                                required = false,
                                item = new[]
                                {
                                    new {
                                        linkId = "LTC_HYT",
                                        text = "Hypertension review",
                                        type = "boolean",
                                        required = false
                                    }
                                }
                            }
                        }
                    },
                    new {
                        resourceType ="Questionnaire",
                        id = "DEFAULT_CONDITION",
                        status = "active",
                        item = new[]
                        {
                            new {
                                linkId = "GEC",
                                text = "General Advice",
                                type = "group",
                                required = false,
                                item = new[]
                                {
                                    new {
                                        linkId = "GEC_GEN__F",
                                        text = "General Advice",
                                        type = "boolean",
                                        required = false
                                    }
                                }
                            },
                        }
                    }
                },
                module = new {
                    reference = "https://test/fhir/ServiceDefinition/CONDITION_LIST"
                },
                status = "data-required",
                dataRequirement = new object[]
                {
                    new
                    {
                        id = "PATIENT",
                        type = "Patient",
                        profile = new [] {
                            "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Patient-1"
                        }
                    },
                    new
                    {
                        id = "ORG",
                        type = "Organization",
                        profile = new [] {
                            "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Organization-1"
                        },
                        codeFilter = new []
                        {
                           new
                           {
                               path = "identifier",
                               valueSetString = "odsOrganizationCode"
                           }
                        }
                    },
                    new
                    {
                        id = "CONDITION_LIST",
                        extension = new []
                        {
                            new
                            {
                                url = "https://www.hl7.org/fhir/questionnaire.html",
                                valueReference = new {
                                   reference = "#CONDITION_LIST"
                                }
                            }
                        },
                        type = "QuestionnaireResponse",
                        profile = new [] {
                            "https://www.hl7.org/fhir/questionnaireresponse.html"
                        }
                    }
                }
            });
        }
    }
}