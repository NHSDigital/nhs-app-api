package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.courses.CoursesData
import features.myrecord.DemographicsData
import mocking.MockDefaults.Companion.patient
import mocking.MockingClient
import mocking.emis.models.*
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient

open class MyRecordStepDefinitions {

  @Steps
  val mockingClient = MockingClient.instance
  val HTTP_EXCEPTION = "HttpException"

  @When("I get the users demographic data with a valid cookie")
  fun whenIGetTheUsersDemographicsWithAValidCookie()
  {
    try {
      mockingClient.forEmis {
        demographicsRequest(patient).respondWithSuccess(DemographicsData.getDemographicData())
      }

      val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getDemographicsConnection(null)

      Serenity.setSessionVariable(DemographicsResponse::class).to(result)
    } catch (httpException: NhsoHttpException) {
      Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
    }
  }

  @Then("I receive the demographic object")
  fun thenIReceiveADemographicObject() {
    val result = Serenity.sessionVariableCalled<DemographicsResponse>(DemographicsResponse::class)
    Assert.assertNotNull(result)
//    Assert.assertEquals(10, result.response.prescriptions.count())
//    val prescriptions = result.response.prescriptions

    // We had to use a string here and then parse the screen as kotlin did not like the date time format sent from the worker
//    for(int in 0 until prescriptions.count()-2){
//      Assert.assertTrue(ZonedDateTime.parse(prescriptions[int].orderDate) !!>= ZonedDateTime.parse(prescriptions[int+1].orderDate))
//    }
  }

}