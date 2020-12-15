package mocking.onlineConsultations.configurations.evaluate

import mocking.onlineConsultations.configurations.IQuestionConfiguration

class SelfOrChildConfiguration : IQuestionConfiguration {
    override val request = """{
      "resourceType":"Parameters",
      "parameter":[
         {
            "name":"inputData",
            "resource":{
               "resourceType":"QuestionnaireResponse",
               "questionnaire":{
                  "reference":"Questionnaire/GLO_PRE_DISCLAIMERS_NHS_2"
               },
               "status":"completed",
               "item":[
                  {
                     "linkId":"GLO_PRE_DISCLAIMERS_NHS_2",
                     "answer":[
                        {
                           "valueCoding":{
                              "code":"GLO_PRE_DISCLAIMERS_1"
                           }
                        }
                     ]
                  }
               ]
            }
         },
         {
            "name":"organization",
            "resource":{
               "resourceType":"Organization",
               "identifier":[
                  {
                     "value":"A29928"
                  }
               ]
            }
         },
         {
            "name":"patient",
            "resource":{
               "resourceType":"Patient",
               "identifier":[
                  {
                     "system":"https://fhir.nhs.uk/Id/nhs-number",
                     "value":"{{nhsNumber}}"
                  }
               ],
               "name":[
                  {
                       "text" : "{{name}} {{familyName}}"
                  }
               ],
               "birthDate":"{{dob}}",
               "address":[
                  {
                     "text":"{{address}}"
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
          "resourceType":"Parameters",
          "id":"outputParams",
          "parameter":[
             {
                "name":"sessionId",
                "valueString":"1"
             }
          ]
        },
        {
          "resourceType": "Questionnaire",
          "id": "PRE_POV_SELF_OR_CHILD",
          "status": "active",
          "item": [
            {
              "linkId": "PRE_POV_SELF_OR_CHILD",
              "text": "Are you asking for help for yourself or for your child?",
              "type": "choice",
              "required": true,
              "repeats": false,
              "option": [
                {
                  "valueCoding": {
                    "code": "PRE_POV_SELF",
                    "display": "For myself"
                  }
                },
                {
                  "valueCoding": {
                    "code": "PRE_POV_CHILD",
                    "display": "For my child"
                  }
                }
              ]
            }
          ]
        }
      ],
      "module": {
        "reference": "https://nhsapp.econsulttest.health/fhir/ServiceDefinition/CONDITION_LIST"
      },
      "status": "data-required",
      "occurrenceDateTime": "2021-02-17T14:35:36.907657249",
      "outputParameters":{
        "reference":"#outputParams"
      },
      "dataRequirement": [
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
