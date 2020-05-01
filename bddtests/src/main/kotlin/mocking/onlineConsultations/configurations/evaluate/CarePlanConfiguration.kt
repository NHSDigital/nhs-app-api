package mocking.onlineConsultations.configurations.evaluate

import mocking.onlineConsultations.configurations.IQuestionConfiguration
import utils.SerenityHelpers

class CarePlanConfiguration: IQuestionConfiguration {

    private val patientFirstName = SerenityHelpers.getPatient().name.firstName

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
                   "reference":"Questionnaire/NHS_ADMIN_AD_REFERRALPAINDURATION"
                },
                "status":"completed",
                "item":[
                   {
                      "linkId":"NHS_ADMIN_AD_REFERRALPAINDURATION",
                      "answer":[
                         {
                            "valueQuantity":{
                               "value":5,
                               "unit":"m"
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
                      "description":"<h1>Thank you $patientFirstName. The answers to your consultation have been 
                        securely sent to your GPs 
                        at Integration Test Practice.</h1><p>We've also emailed you a confirmation of your consultation.
                        </p><h2>What happens next?</h2><ul><li>One of your GPs will now review your consultation.</li>
                        <li>Your practice will respond to you by 
                        phone or email <strong>before 6:30 PM on 28 February 2020</strong>.</li></ul>
                        <div class='messageBox alert 
                        intro-text'>If your condition gets worse while you're waiting for a response, call the practice 
                        on 01234567890 as soon as possible. If the practice is closed, call NHS 111. For immediate, 
                        life-threatening emergencies, call 999
                        .<p/></div><p><strong>Here are two important tips for ensuring you receive a 
                        response:</strong></p><ul><li>A call from
                         your practice may sometimes appear on your phone as 'Blocked','No caller ID' or 'Private 
                         number'</li> <li>Check your
                        junk email folder</li></ul><h2>What if I don’t get a call or email?</h2><p>In most cases  you 
                        won't need to contact 
                        the practice <strong>before 6:30 PM on 28 February 2020.</strong></p><p>There is generally no 
                        need for you to 
                        contact the practice before 6:30 PM on 28 February 2020. But if they're unable to reach you 
                        by then, please let them
                         know by using the link in your confirmation email or by calling the practice on <strong> 
                         01234567890</strong>. 
                        </p><h2>What if I'm given a prescription?</h2><p>If your doctor decides you need a 
                        prescription you will be able to 
                        pick it up from the practice or your local pharmacy.</p><h2>I think I made a mistake 
                         on my consultation! What do I do
                         now?</h2><p>Please call the practice on <strong>01234567890</strong>. A member of 
                         staff will be able to update your 
                        record </p>"
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
       "occurrenceDateTime":"2020-02-27T08:28:49.308",
       "outputParameters":{
          "reference":"#outputParams"
       },
       "result":{
          "reference":"#rg1"
       }
    }"""
}