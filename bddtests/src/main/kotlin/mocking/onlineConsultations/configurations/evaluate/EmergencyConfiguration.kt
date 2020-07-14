package mocking.onlineConsultations.configurations.evaluate

import mocking.onlineConsultations.configurations.IQuestionConfiguration

class EmergencyConfiguration: IQuestionConfiguration {
    override val request = """{
       "resourceType":"Parameters",
       "parameter":[
          {
             "name":"sessionId",
             "valueString":"05fc67a8-3192-4e27-9ef2-850b07add991"
          },
          {
             "name":"inputData",
             "resource":{
                "resourceType":"QuestionnaireResponse",
                "status":"completed",
                "item":[
                   {
                      "linkId":"PRE_STD_AD_EMERGENCY",
                      "answer":[
                         {
                            "valueCoding":{
                               "code":"PRE_STD_EMERGENCY_YES"
                            }
                         }
                      ]
                   }
                ],
                "questionnaire":{
                   "reference":"Questionnaire/PRE_STD_AD_EMERGENCY"
                }
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
             "id":"ADVICE_EMERGENCY",
             "status":"active",
             "item":[
                {
                   "linkId":"ADVICE_EMERGENCY_GROUP",
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
                         "linkId":"ADVICE_EMERGENCY_PREV",
                         "text":"",
                         "type":"boolean",
                         "required":false
                      },
                      {
                         "linkId":"ADVICE_EMERGENCY",
                         "text":"<div class='nhsuk-care-card nhsuk-care-card--immediate'> 
                <div class='nhsuk-care-card__heading-container'> <h3 class='nhsuk-carecard__heading'>
                <span role='text'> <span class='nhsuk-u-visually-hidden'>Emergency advice:</span>
                You need medical help now. Call 999 or go to your local A&E.</span></h3> 
                <span class='nhsuk-care-card__arrow' aria-hidden='true'></span> 
                </div><div class='nhsuk-care-card__content'><p>If you decide to seek medical advice, 
                <strong>your GP will not be notified and your practice will not contact you about this consultation.
                </strong></p></div></div>",
                         "type":"group",
                         "required":true,
                         "item":[
                            {
                               "linkId":"ADVICE_EMERGENCY_CONFIRM",
                               "text":"End my consultation, I will seek medical advice myself instead",
                               "type":"boolean",
                               "required":true
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
       "occurrenceDateTime":"2020-02-25T08:45:21.976",
       "outputParameters":{
          "reference":"#outputParams"
       },
       "dataRequirement":[
          {
             "id":"ADVICE_EMERGENCY",
             "extension":[
                {
                   "url":"https://www.hl7.org/fhir/questionnaire.html",
                   "valueReference":{
                      "reference":"#ADVICE_EMERGENCY"
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
