package mocking.onlineConsultations.configurations.evaluate

import mocking.onlineConsultations.configurations.IQuestionConfiguration

class TermsAndConditionsQuestionConfigurationI: IQuestionConfiguration {
    
    override val request = """{
       "resourceType":"Parameters",
       "parameter":[
          {
             "name":"organization",
             "resource":{
                "resourceType":"Organization",
                "identifier":{
                   "value":"{{odsCode}}"
                }
             }
          }
       ]
    }"""

    override val response = """{
       "resourceType":"GuidanceResponse",
       "contained":[
          {
             "resourceType":"Parameters",
             "id":"outputParams"
          },
          {
             "resourceType":"Questionnaire",
             "id":"GLO_PRE_DISCLAIMERS_NHS_2",
             "status":"active",
             "item":[
                {
                   "linkId":"GLO_PRE_DISCLAIMERS_NHS_2",
                   "text":"<p>We're about to ask you a few questions about your request.
            Your answers will be sent securely to your GP surgery unless urgent medical attention is needed. 
            For such cases the online consultation service will direct you to other health services.</p><br/>
            <p>To start, please agree to the privacy notice applicable to online consultation services.</p>",
                   "type":"group",
                   "required":true,
                   "item":[
                      {
                         "linkId":"GLO_PRE_DISCLAIMERS_1",
                         "text":"I have read the
            <a href='https://stubs.onlineconsultations/staticLegalContent/nhsAppPrivacyPolicy' 
            target='_blank'>GP online consultation services privacy notice.</a>
            understand the online consultation service provider will process my personal and health data on behalf of 
            my GP surgery to provide an online consultation.",
                         "type":"boolean",
                         "required":true
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
       "occurrenceDateTime":"2020-02-21T17:04:45.098",
       "outputParameters":{
          "reference":"#outputParams"
       },
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
             "id":"GLO_PRE_DISCLAIMERS_NHS_2",
             "extension":[
                {
                   "url":"https://www.hl7.org/fhir/questionnaire.html",
                   "valueReference":{
                      "reference":"#GLO_PRE_DISCLAIMERS_NHS_2"
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
