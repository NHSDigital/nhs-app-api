package features.myrecord.stepDefinitions

import constants.ErrorResponseCodeTpp
import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.linkedProfiles.LinkedProfilesSerenityHelpers
import mocking.data.myrecord.TppDcrData
import mocking.tpp.models.Error
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import utils.SerenityHelpers
import utils.getOrFail
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

open class MyRecordTppDcrEventStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Given("^the GP Practice has multiple dcr events for TPP$")
    fun givenTheGpPracticeHasMultipleDcrEventsForTpp() {
        setPatientToDefaultFor(Supplier.TPP)
        mockingClient.forTpp {
            myRecord.patientRecordRequest(SerenityHelpers.getPatient().tppUserSession!!)
                    .respondWithSuccess(TppDcrData.getMultipleDcrEventsForTpp())
        }
    }

    @Given("^the GP Practice has disabled dcr events functionality for TPP$")
    fun givenThePatientDoesNotHaveAccessToDcrEventsForTPP() {
        setPatientToDefaultFor(Supplier.TPP)
        mockingClient.forTpp {
            myRecord.patientRecordRequest(SerenityHelpers.getPatient().tppUserSession!!)
                    .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                            "You don&apos;t have access to this online service. " +
                                    "You can request access to this service at Kainos GP Demo Unit by " +
                                    "clicking Manage Online Services in the Account section.",
                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }

    @Given("^an error occurred retrieving the dcr events from TPP$")
    fun givenAnErrorOccurredRetrievingDcrEventsFromTPP() {
        setPatientToDefaultFor(Supplier.TPP)
        mockingClient.forTpp {
            myRecord.patientRecordRequest(SerenityHelpers.getPatient().tppUserSession!!)
                    .respondWithServiceNotAvailableException()
        }
    }

    @When("^I get the users dcr event data$")
    fun whenIGetTheUsersMyRecordData() {
        try {
            val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .myRecord.getMyRecord(patientId)

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I receive \"(.*)\" dcr events as part of the my record object$")
    fun thenIReceiveATestResultsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.tppDcrEvents.data.count())
    }

    @Then("^the flag informing that the patient has access to the dcr event data is set to \"(.*)\"$")
    fun thenTheFlagInformingThePatientHasAccessToDcrEventDataIs(hasAccess: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(hasAccess, result.response.tppDcrEvents.hasAccess)
    }

    @Then("^the flag informing that there was an error retrieving the dcr event data is set to \"(.*)\"$")
    fun thenTheFlagInformingAnErrorOccurredGettingTheDcrEventDataIs(hasAccess: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(hasAccess, result.response.tppDcrEvents.hasErrored)
    }
}

