package mocking.onlineConsultations.configurations

class AlcoholQuestionConfiguration: IQuestionConfiguration {

    override val request: String = """{
       "resourceType":"Parameters",
       "parameter":[
          {
             "name":"sessionId",
             "valueString":"4ea9390f-f6f2-410b-8d46-39b37a6a1753"
          },
          {
             "name":"inputData",
             "resource":{
                "resourceType":"QuestionnaireResponse",
                "questionnaire":{
                   "reference":"Questionnaire/PRE_STD_AD_DOB"
                },
                "status":"completed",
                "item":[
                   {
                      "linkId":"PRE_STD_AD_DOB",
                      "answer":[
                         {
                            "valueDate":"1972-04-12"
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
                   "valueString":"0d7036ed-3c5a-48a8-9927-81dbcf50e6a6"
                }
             ]
          },
          {
             "resourceType":"Questionnaire",
             "id":"Q_GEC_ADM_AD_80",
             "status":"active",
             "item":[
                {
                   "linkId":"Q_GEC_ADM_AD_80_GROUP",
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
                         "linkId":"Q_GEC_ADM_AD_80_PREV",
                         "text":"",
                         "type":"boolean",
                         "required":false
                      },
                      {
                         "linkId":"Q_GEC_ADM_AD_80",
                         "text":"Do you know how many units of alcohol you drink each week? 
                 - 1 pint of beer is approximately two units and one small glass of wine is 1 unit",
                         "type":"choice",
                         "required":true,
                         "repeats":false,
                         "option":[
                            {
                               "valueCoding":{
                                  "code":"A_GEC_ADM_AD_139",
                                  "display":"Don't know"
                               }
                            },
                            {
                               "valueCoding":{
                                  "code":"A_GEC_ADM_AD_140",
                                  "display":"Zero"
                               }
                            },
                            {
                               "valueCoding":{
                                  "code":"A_GEC_ADM_AD_141",
                                  "display":"1-7 units per week"
                               }
                            },
                            {
                               "valueCoding":{
                                  "code":"A_GEC_ADM_AD_142",
                                  "display":"8-14 units per week"
                               }
                            },
                            {
                               "valueCoding":{
                                  "code":"A_GEC_ADM_AD_143",
                                  "display":"15-21 units per week"
                               }
                            },
                            {
                               "valueCoding":{
                                  "code":"A_GEC_ADM_AD_144",
                                  "display":"22-28 units per week"
                               }
                            },
                            {
                               "valueCoding":{
                                  "code":"A_GEC_ADM_AD_145",
                                  "display":"29-35 units per week"
                               }
                            },
                            {
                               "valueCoding":{
                                  "code":"A_GEC_ADM_AD_146",
                                  "display":"36-69 units per week"
                               }
                            },
                            {
                               "valueCoding":{
                                  "code":"A_GEC_ADM_AD_147",
                                  "display":"More than 70 units per week"
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
          "reference":"https://stubs.onlineconsultations/fhir/ServiceDefinition/GEC_ADM"
       },
       "status":"data-required",
       "occurrenceDateTime":"2020-02-25T13:32:17.208",
       "outputParameters":{
          "reference":"#outputParams"
       },
       "dataRequirement":[
          {
             "id":"Q_GEC_ADM_AD_80",
             "extension":[
                {
                   "url":"https://www.hl7.org/fhir/questionnaire.html",
                   "valueReference":{
                      "reference":"#Q_GEC_ADM_AD_80"
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