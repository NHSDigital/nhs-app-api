package mocking.onlineConsultations.configurations.evaluate.childJourneyUnder18

import mocking.onlineConsultations.configurations.IQuestionConfiguration

class YouMustBeOver18Message: IQuestionConfiguration {
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

    override val response = """{
       "resourceType":"GuidanceResponse",
       "contained":[
          {
             "resourceType":"Parameters",
             "id":"outputParams",
             "parameter":[
                {
                   "name":"sessionId",
                   "valueString":"6ad97e5b-c498-4ba6-9135-6bc9167846d0"
                }
             ]
          },
          {
             "resourceType":"CarePlan",
             "id":"careplan1",
             "status":"active",
             "intent":"option",
             "title":"",
             "activity":[
                {
                   "detail":{
                      "description":"You must be between 18 and 125 years old in order to complete this request."
                   }
                }
             ]
          },
          {
             "resourceType":"RequestGroup",
             "id":"rg1",
             "action":[
                {
                   "resource":{
                      "reference":"#careplan1"
                   }
                }
             ]
          }
       ],
       "module":{
          "reference":"https://stubs.onlineconsultations/fhir/ServiceDefinition/"
       },
       "status":"success",
       "occurrenceDateTime":"2020-02-27T08:28:49.308",
       "outputParameters":{
          "reference":"#outputParams"
       },
       "result":{
          "reference":"#rg1"
       }
    }"""
}
