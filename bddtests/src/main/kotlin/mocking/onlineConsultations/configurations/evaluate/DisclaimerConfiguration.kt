package mocking.onlineConsultations.configurations.evaluate

import mocking.onlineConsultations.configurations.IQuestionConfiguration

class DisclaimerConfiguration : IQuestionConfiguration {
    override val request  = """{
       "resourceType":"Parameters",
       "parameter":[
        {
          "name":"sessionId",
          "valueString":"1"
        },
        {
          "name": "inputData",
          "resource": {
              "resourceType": "QuestionnaireResponse",
              "questionnaire": {
                "reference": "Questionnaire/PRE_POV_SELF_OR_CHILD"
              },
              "status": "completed",
              "item": [
                  {
                      "linkId": "PRE_POV_SELF_OR_CHILD",
                      "answer": [
                          {
                              "valueCoding": {
                                  "code": "PRE_POV_SELF"
                              }
                          }
                      ]
                  }
              ]
          }
        },
        {
          "name": "organization",
          "resource": {
              "resourceType": "Organization",
              "identifier": [
                {
                  "value": "{{odsCode}}"
                }
              ]
          }
        } 
       ]
    }"""

    override val response = """{
    "resourceType": "GuidanceResponse",
    "contained": [
        {
            "resourceType": "Questionnaire",
            "id": "GLO_PRE_DISCLAIMERS_NHS_2",
            "status": "active",
            "item": [
                {
                    "linkId": "GLO_PRE_DISCLAIMERS_NHS_2",
                    "text": "We're about to ask you a few questions about your request.",
                    "type": "group",
                    "required": true,
                    "item": [
                        {
                            "linkId": "GLO_PRE_DISCLAIMERS_1",
                            "text": "I have read the information.",
                            "type": "boolean",
                            "required": true
                        }
                    ]
                }
            ]
        }
    ],
    "requestId": "3",
    "module": {
        "reference": "https://nhsapp.econsulttest.health/fhir/ServiceDefinition/CONDITION_LIST"
    },
    "status": "data-required",
    "occurrenceDateTime": "2020-12-16T13:51:46.628",
    "dataRequirement": [
        {
            "id": "PATIENT",
            "type": "Patient",
            "profile": [
                "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Patient-1"
            ]
        },
        {
            "id": "ORG",
            "type": "Organization",
            "profile": [
                "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Organization-1"
            ],
            "codeFilter": [
                {
                    "path": "identifier",
                    "valueSetString": "odsOrganizationCode"
                }
            ]
        },
        {
            "id": "GLO_PRE_DISCLAIMERS_NHS_2",
            "extension": [
                {
                    "url": "https://www.hl7.org/fhir/questionnaire.html",
                    "valueReference": {
                        "reference": "#GLO_PRE_DISCLAIMERS_NHS_2"
                    }
                }
            ],
            "type": "QuestionnaireResponse",
            "profile": [
                "https://www.hl7.org/fhir/questionnaireresponse.html"
            ]
        },
        {
            "id": "PRE_POV_SELF_OR_CHILD",
            "extension": [
                {
                    "url": "https://www.hl7.org/fhir/questionnaire.html",
                    "valueReference": {
                        "reference": "#PRE_POV_SELF_OR_CHILD"
                    }
                }
            ],
            "type": "QuestionnaireResponse",
            "profile": [
                "https://www.hl7.org/fhir/questionnaireresponse.html"
            ]
        }
    ]
}"""

}
