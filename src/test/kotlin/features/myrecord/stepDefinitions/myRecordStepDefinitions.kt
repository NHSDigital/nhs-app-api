package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.DemographicsData
import features.myrecord.AllergiesData
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
  }


  @When("I get the users allergy data with a valid cookie")
  fun whenIGetTheUsersAllergiesWithAValidCookie()
  {
    try {
      mockingClient.forEmis {
        allergiesRequest(patient).respondWithSuccess(AllergiesData.getAllergiesData())
      }

      val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getAllergiesConnection(null)

      Serenity.setSessionVariable(AllergiesResponse::class).to(result)
    } catch (httpException: NhsoHttpException) {
      Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
    }
  }

  @Then("I receive the allergies object")
  fun thenIReceiveAnAllergiesObject() {
    val result = Serenity.sessionVariableCalled<AllergiesResponse>(AllergiesResponse::class)
    Assert.assertNotNull(result)
  }
}