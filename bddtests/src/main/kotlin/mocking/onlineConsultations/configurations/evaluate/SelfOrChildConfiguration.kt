package mocking.onlineConsultations.configurations.evaluate

import mocking.onlineConsultations.configurations.IQuestionConfiguration

class SelfOrChildConfiguration : IQuestionConfiguration {

    override val request: String  = """{
       "resourceType":"Parameters",
       "parameter":[
          {
             "name":"sessionId",
             "valueString":"1"
          },
          {
             "name":"inputData",
             "resource":{
                "resourceType":"QuestionnaireResponse",
                "questionnaire":{
                   "reference":"Questionnaire/PRE_STD_AD_SEX"
                },
                "status":"completed",
                "item":[
                   {
                      "linkId":"PRE_STD_AD_SEX",
                      "answer":[
                         {
                            "valueCoding":{
                               "code":"PRE_STD_AD_SEX_M"
                            }
                         }
                      ]
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
             "resourceType":"Parameters",
             "id":"outputParams",
             "parameter":[
                {
                   "name":"sessionId",
                   "valueString":"f59d8846-2968-45f7-8ab9-8acc57ebd858"
                }
             ]
          },
          {
             "resourceType":"Questionnaire",
             "id":"PRE_STD_AD_SELFONLY",
             "status":"active",
             "item":[
                {
                   "linkId":"PRE_STD_AD_SELFONLY",
                   "text":"<p>Please confirm that you're submitting this request 
                for yourself.</p><p>If you need help for your child or someone you care for,
                <a href='https://nhsapptest.econsulttest.health/'>visit the practice homepage</a> 
                to find out how to book an appointment instead.</p>",
                   "type":"choice",
                   "required":true,
                   "repeats":true,
                   "option":[
                      {
                         "valueCoding":{
                            "code":"PRE_STD_AD_SELFONLY_SELF",
                            "display":"I confirm I'm submitting this request for myself, 
                not someone else (like my child)"
                         }
                      }
                   ]
                }
             ]
          }
       ],
       "module":{
          "reference":"https://stubs.onlineconsultations/fhir/ServiceDefinition/BRP_BRP"
       },
       "status":"data-required",
       "occurrenceDateTime":"2020-02-24T18:01:55.244",
       "outputParameters":{
          "reference":"#outputParams"
       },
       "dataRequirement":[
          {
             "id":"PRE_STD_AD_SELFONLY",
             "extension":[
                {
                   "url":"https://www.hl7.org/fhir/questionnaire.html",
                   "valueReference":{
                      "reference":"#PRE_STD_AD_SELFONLY"
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