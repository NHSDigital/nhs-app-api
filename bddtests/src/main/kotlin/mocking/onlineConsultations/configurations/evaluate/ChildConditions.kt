package mocking.onlineConsultations.configurations.evaluate

import mocking.onlineConsultations.configurations.IQuestionConfiguration

class ChildConditions: IQuestionConfiguration {
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
                               "code": "PRE_POV_CHILD"
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

    override val response: String = """{
       "resourceType":"GuidanceResponse",
       "contained":[
          {
             "resourceType":"Questionnaire",
             "id":"CONDITION_LIST",
             "status":"active",
             "item":[
                {
                   "linkId":"BRP",
                   "text":"Breathing problems",
                   "type":"group",
                   "required":false,
                   "item":[
                      {
                         "linkId":"BRP_BRP",
                         "text":"Breathing problems for child",
                         "type":"boolean",
                         "required":false
                      },
                   ]
                },
                {
                   "linkId":"TEST_CONDITION",
                   "text":"Test child condition",
                   "type":"group",
                   "required":false,
                   "item":[
                      {
                         "linkId":"TEST_CONDITION",
                         "text":"Test child condition",
                         "type":"boolean",
                         "required":false
                      },
                   ]
                }
             ]
          },
          {
            "resourceType": "Questionnaire",
            "id": "DEFAULT_CONDITION",
            "status": "active",
            "item": [
                {
                    "linkId": "PCI_UWC",
                    "text": "General Advice",
                    "type": "group",
                    "required": false,
                    "item": [
                        {
                            "linkId": "PCI_UWC__T",
                            "text": "General Advice",
                            "type": "boolean",
                            "required": false
                        }
                    ]
                }
            ]
        }
       ],
       "module":{
          "reference":"https://test/fhir/ServiceDefinition/CONDITION_LIST"
       },
       "status":"data-required",
       "dataRequirement":[
          {
             "id":"PATIENT",
             "type":"Patient",
             "profile":[
                "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Patient-1"
             ]
          },
          {
             "id":"ORG",
             "type":"Organization",
             "profile":[
                "https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Organization-1"
             ],
             "codeFilter":[
                {
                   "path":"identifier",
                   "valueSetString":"odsOrganizationCode"
                }
             ]
          },
          {
             "id":"CONDITION_LIST",
             "extension":[
                {
                   "url":"https://www.hl7.org/fhir/questionnaire.html",
                   "valueReference":{
                      "reference":"#CONDITION_LIST"
                   }
                }
             ],
             "type":"QuestionnaireResponse",
             "profile":[
                "https://www.hl7.org/fhir/questionnaireresponse.html"
             ]
          }
       ]
    }"""
}
