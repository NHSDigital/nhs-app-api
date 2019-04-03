package features.myrecord.stepDefinitions

import constants.ErrorResponseCodeTpp
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.data.myrecord.ConsultationsData
import mocking.data.myrecord.TppDcrData
import mocking.defaults.EmisMockDefaults
import mocking.tpp.models.Error
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

open class MyRecordConsultationStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Given("^the GP Practice has multiple consultations$")
    fun givenTheGpPracticeHasMultipleConsultationsFor() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    myRecord.consultationsRequest(EmisMockDefaults.patientEmis)
                            .respondWithSuccess(ConsultationsData.getMultipleConsultationRecords())
                }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    myRecord.patientRecordRequest(this@MyRecordConsultationStepDefinitions.patient.tppUserSession!!)
                            .respondWithSuccess(TppDcrData.getMultipleDcrEventsForTpp())
                }
            }
        }
    }

    @Given("^the Patient has no access to consultations$")
    fun givenTheGPPracticeHasNoAccessToConsultations() {
        when (SerenityHelpers.getGpSupplier()) {
            "EMIS" -> {
                mockingClient.forEmis {
                    myRecord.consultationsRequest(EmisMockDefaults.patientEmis).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP" -> {
                setPatientToDefaultFor("TPP")
                mockingClient.forTpp {
                    myRecord.patientRecordRequest(this@MyRecordConsultationStepDefinitions.patient.tppUserSession!!)
                            .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                                    "You don&apos;t have access to this online service. " +
                                            "You can request access to this service at Kainos GP Demo Unit by " +
                                            "clicking Manage Online Services in the Account section.",
                                    "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                }

            }
        }
    }

    @Given("^the GP Practice has no consultations$")
    fun givenThePracticeHasNoConsultations() {
        when (SerenityHelpers.getGpSupplier()) {
            "EMIS" -> {
                mockingClient.forEmis {
                    myRecord.consultationsRequest(EmisMockDefaults.patientEmis)
                            .respondWithSuccess(ConsultationsData.getDefaultConsultationsData())
                }
            }
            "TPP" -> {
                setPatientToDefaultFor("TPP")
                mockingClient.forTpp {
                    myRecord.patientRecordRequest(this@MyRecordConsultationStepDefinitions.patient.tppUserSession!!)
                            .respondWithSuccess(TppDcrData.getDefaultTppDcrData())

                }

            }
        }
    }

    @Given("^an error occurred retrieving the consultations$")
    fun givenAnErrorOccurredRetrievingTestResults() {
        when (SerenityHelpers.getGpSupplier()) {
            "EMIS" -> {
                mockingClient.forEmis {
                    myRecord.consultationsRequest(EmisMockDefaults.patientEmis).respondWithNonDataAccessException()
                }
            }
            "TPP" -> {
                setPatientToDefaultFor("TPP")
                mockingClient.forTpp {
                    myRecord.patientRecordRequest(this@MyRecordConsultationStepDefinitions.patient.tppUserSession!!)
                            .respondWithServiceNotAvailableException()
                }

            }
        }
    }

    @When("I get the users consultations")
    fun whenIGetTheUsersMyRecordData() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).myRecord.getMyRecord()

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("I receive \"(.*)\" consultations as part of the my record object")
    fun thenIReceiveATestResultsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.consultations.data.count())
    }

    @Then("^I receive consultations object with hasAccess flag set to \"(.*)\"$")
    fun thenIReceiveConsultationsWithHasAccessFlagSetTo(hasAccess: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(hasAccess, result.response.consultations.hasAccess)
    }

    @Then("^the consultations object with hasErrored flag set to \"(.*)\"$")
    fun thenIReceiveConsultationsWithErrorFlagSetTo(hasError: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(hasError, result.response.consultations.hasErrored)
    }

    @Then("^I see Consultations records displayed$")
    fun thenISeeConsultationsRecordsDisplayed() {
        assertEquals(2, myRecordInfoPage.consultations.allRecordItems().count())
    }
}

