package features.myrecord.stepDefinitions

import cucumber.api.java.en.*
import mocking.defaults.MockDefaults.Companion.patient
import mocking.MockingClient
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.myrecord.*
import features.myrecord.steps.MyRecordSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.defaults.MockDefaults
import mocking.emis.models.MedicationsResponse
import worker.models.demographics.DemographicsResponse
import worker.models.myrecord.MyRecordResponse
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient

open class MyRecordImmunisationStepDefinitions {

    @Steps
    val mockingClient = MockingClient.instance
    val HTTP_EXCEPTION = "HttpException"

    @Given("the GP Practice has enabled immunisations functionality and multiple immunisation records exist")
    fun givenTheGPPracticeHasEnabledImmunisationsFunctionalityAndMultipleImmunisationRecordsExist() {

        mockingClient.forEmis {
            testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getMultipleTestResultsData())
        }

        mockingClient.forEmis {
            immunisationsRequest(patient).respondWithSuccess(ImmunisationsData.getImmunisationsData())
        }

        mockingClient.forEmis {
            allergiesRequest(patient).respondWithSuccess(AllergiesData.getAllergiesData(2))
        }

        mockingClient.forEmis {
            medicationsRequest(patient).respondWithSuccess(MedicationsData.getMedicationData())
        }
    }

    @Given("no immunisation records exist for the patient")
    fun givenNoImmunisationRecordsExistForThePatient() {

        mockingClient.forEmis {
            testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getMultipleTestResultsData())
        }

        mockingClient.forEmis {
            immunisationsRequest(patient).respondWithSuccess(ImmunisationsData.getEmptyImmunisationsData())
        }

        mockingClient.forEmis {
            allergiesRequest(patient).respondWithSuccess(AllergiesData.getAllergiesData(2))
        }

        mockingClient.forEmis {
            medicationsRequest(patient).respondWithSuccess(MedicationsData.getMedicationData())
        }
    }

    @Given("the user does not have access to view immunisations")
    fun givenUserDoesNotHaveAccessToViewImmunisations() {

        mockingClient.forEmis {
            testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getMultipleTestResultsData())
        }

        mockingClient.forEmis {
            immunisationsRequest(patient).respondWithExceptionWhenNotEnabled()
        }

        mockingClient.forEmis {
            allergiesRequest(patient).respondWithSuccess(AllergiesData.getAllergiesData(2))
        }

        mockingClient.forEmis {
            medicationsRequest(patient).respondWithSuccess(MedicationsData.getMedicationData())
        }
    }

    @Given("there is an error retrieving immunisations data")
    fun givenThereIsAnErrorRetrievingImmunisationsData() {

        mockingClient.forEmis {
            testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getMultipleTestResultsData())
        }

        mockingClient.forEmis {
            immunisationsRequest(patient).respondWithNonDataAccessException()
        }

        mockingClient.forEmis {
            allergiesRequest(patient).respondWithSuccess(AllergiesData.getAllergiesData(2))
        }

        mockingClient.forEmis {
            medicationsRequest(patient).respondWithSuccess(MedicationsData.getMedicationData())
        }
    }
    
    @When("I get the users immunisations")
    fun whenIGetTheUsersMyRecordData()
    {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getMyRecord(null)

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("I receive \"(.*)\" immunisations as part of the my record object")
    fun thenIReceiveAnImmunisationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.immunisations.data.count())
    }
}