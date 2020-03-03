package mocking.onlineConsultations.configurations

class UrgencyQuestionConfiguration : IQuestionConfiguration {

    override val request: String = """{
       "resourceType":"Parameters",
       "parameter":[
          {
             "name":"sessionId",
             "valueString":"f59d8846-2968-45f7-8ab9-8acc57ebd858"
          },
          {
             "name":"inputData",
             "resource":{
                "resourceType":"QuestionnaireResponse",
                "questionnaire":{
                   "reference":"Questionnaire/PRE_STD_AD_SELFONLY"
                },
                "status":"completed",
                "item":[
                   {
                      "linkId":"PRE_STD_AD_SELFONLY",
                      "answer":[
                         {
                            "valueCoding":{
                               "code":"PRE_STD_AD_SELFONLY_SELF"
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
                   "valueString":"05fc67a8-3192-4e27-9ef2-850b07add991"
                }
             ]
          },
          {
             "resourceType":"Questionnaire",
             "id":"PRE_STD_AD_EMERGENCY",
             "status":"active",
             "item":[
                {
                   "linkId":"PRE_STD_AD_EMERGENCY_GROUP",
                   "text":"",
                   "type":"group",
                   "item":[
                      {
                         "extension":[
                            {
                               "url":"http://hl7.org/fhir/StructureDefinition/questionnaire-itemControl",
                               "valueCodeableConcept":{
                                  "id":"backButton",
                                  "coding":[
                                     {
                                        "system":"http://hl7.org/fhir/ValueSet/questionnaire-item-control",
                                        "code":"button",
                                        "display":"back"
                                     }
                                  ],
                                  "text":"back"
                               }
                            }
                         ],
                         "linkId":"PRE_STD_AD_EMERGENCY_PREV",
                         "text":"",
                         "type":"boolean",
                         "required":false
                      },
                      {
                         "linkId":"PRE_STD_AD_EMERGENCY",
                         "text":"<div>
                <p><b>Next, let's make sure this isn't an emergency.
                Are you currently experiencing any of the following?</b></p><ul><li> 
                <span>
                Signs of a heart attack</span> - pain like a very tight band, heavy weight or squeezing 
                in the centre of your chest or any pain that moves into your jaw or neck</li><li>
                <span>Signs of a stroke</span> -
                face drooping on one side, can't hold both arms up, difficulty speaking, or weakness or numbness 
                on one side of your body</li><li><span>Severe difficulty breathing</span> - 
                gasping, not being able to get words out, choking or lips turning blue</li> 
                <li><span>
                Heavy bleeding that won't stop</span> - uncontrollable bleeding from any part of your body</li>
                <li><span>Severe injuries</span> - 
                including deep cuts after a serious accident</li></ul></div>",
                         "type":"choice",
                         "required":true,
                         "repeats":false,
                         "option":[
                            {
                               "valueCoding":{
                                  "code":"PRE_STD_EMERGENCY_NO",
                                  "display":"I'm NOT experiencing any of these"
                               }
                            },
                            {
                               "valueCoding":{
                                  "code":"PRE_STD_EMERGENCY_YES",
                                  "display":"I am experiencing some of these"
                               }
                            }
                         ]
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
       "occurrenceDateTime":"2020-02-25T08:31:00.851",
       "outputParameters":{
          "reference":"#outputParams"
       },
       "dataRequirement":[
          {
             "id":"PRE_STD_AD_EMERGENCY",
             "extension":[
                {
                   "url":"https://www.hl7.org/fhir/questionnaire.html",
                   "valueReference":{
                      "reference":"#PRE_STD_AD_EMERGENCY"
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