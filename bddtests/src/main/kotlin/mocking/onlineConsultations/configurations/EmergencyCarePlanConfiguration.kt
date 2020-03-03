package mocking.onlineConsultations.configurations

class EmergencyCarePlanConfiguration: IQuestionConfiguration {

    override val request: String = """{
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
                      "linkId":"ADVICE_EMERGENCY",
                      "answer":[
                         {
                            "valueCoding":{
                               "code":"ADVICE_EMERGENCY_CONFIRM"
                            }
                         }
                      ]
                   }
                ],
                "questionnaire":{
                   "reference":"Questionnaire/ADVICE_EMERGENCY"
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
             "resourceType":"CarePlan",
             "id":"careplan1",
             "status":"active",
             "intent":"option",
             "title":"",
             "activity":[
                {
                   "detail":{
                      "description":"<div class='nhsuk-care-card nhsuk-care-card--immediate'>
                        <div class='nhsuk-care-card__heading-container'>
                        <h3 class='nhsuk-care-card__heading'><span role='text'><span class='nhsuk-u-visually-hidden'>
                        Emergency advice:</span> You've chosen to end your consultation. 
                        Your practice hasn't been notified and won't contact you about your request. 
                        You should still seek medical advice now.</span></h3><span class='nhsuk-care-card__arrow'
                        aria-hidden='true'></span> </div><div class='nhsuk-care-card__content'> 
                        <p>Here's what you can do next:</p><ul><li>Call 999</li><li>Go to your local A&E</li></ul> 
                        </div></div>"
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
          "reference":"https://stubs.onlineconsultations/fhir/ServiceDefinition/BRP_BRP"
       },
       "status":"success",
       "occurrenceDateTime":"2020-02-25T08:48:02.212",
       "outputParameters":{
          "reference":"#outputParams"
       },
       "result":{
          "reference":"#rg1"
       }
    }"""
}