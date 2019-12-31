package features.myrecord.stepDefinitions

import constants.ErrorResponseCodeTpp
import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.linkedProfiles.LinkedProfilesSerenityHelpers
import mocking.data.myrecord.ConsultationsData
import mocking.data.myrecord.TppDcrData
import mocking.tpp.models.Error
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import pages.myrecord.MedicalRecordV1Page
import utils.SerenityHelpers
import utils.getOrFail
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse
import java.lang.UnsupportedOperationException

open class V1MedicalRecordConsultationStepDefinitions : AbstractDemographicsStepDefinitions() {

    private lateinit var medicalRecordV1Page: MedicalRecordV1Page

    @Given("^the GP Practice has multiple consultations$")
    fun givenTheGpPracticeHasMultipleConsultations() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatient()

        when (gpSystem) {
            Supplier.EMIS -> {
                mockingClient.forEmis {
                    myRecord.consultationsRequest(patient)
                            .respondWithSuccess(ConsultationsData.getMultipleConsultationRecords())
                }
            }
            Supplier.TPP -> {
                mockingClient.forTpp {
                    myRecord.patientRecordRequest(patient.tppUserSession!!)
                            .respondWithSuccess(TppDcrData.getMultipleDcrEventsForTpp())
                }
            }
            else -> throw UnsupportedOperationException("Not implemented for $gpSystem")
        }
    }

    @Given("^the Patient has no access to consultations$")
    fun givenThePatientHasNoAccessToConsultations() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatient()

        when (gpSystem) {
            Supplier.EMIS -> {
                mockingClient.forEmis {
                    myRecord.consultationsRequest(patient).respondWithExceptionWhenNotEnabled()
                }
            }
            Supplier.TPP -> {
                mockingClient.forTpp {
                    myRecord.patientRecordRequest(patient.tppUserSession!!)
                            .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                                    "You don&apos;t have access to this online service. " +
                                            "You can request access to this service at Kainos GP Demo Unit by " +
                                            "clicking Manage Online Services in the Account section.",
                                    "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                }

            }
            else -> throw UnsupportedOperationException("Not implemented for $gpSystem")
        }
    }

    @Given("^the GP Practice has no consultations$")
    fun givenThePracticeHasNoConsultations() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatient()

        when (gpSystem) {
            Supplier.EMIS -> {
                mockingClient.forEmis {
                    myRecord.consultationsRequest(patient)
                            .respondWithSuccess(ConsultationsData.getDefaultConsultationsData())
                }
            }
            Supplier.TPP -> {
                mockingClient.forTpp {
                    myRecord.patientRecordRequest(patient.tppUserSession!!)
                            .respondWithSuccess(TppDcrData.getDefaultTppDcrData())

                }

            }
            else -> throw UnsupportedOperationException("Not implemented for $gpSystem")
        }
    }

    @Given("^the EMIS GP Practice has two consultations where the second record has no date$")
    fun givenTheEmisPracticeHasAConsultationWithNoDate(){
        val patient = SerenityHelpers.getPatient()

        mockingClient.forEmis {
            myRecord.consultationsRequest(patient)
                    .respondWithSuccess(ConsultationsData.getTwoConsultationsWhereTheSecondRecordHasNoDate())
        }
    }

    @Given("^an error occurred retrieving the consultations$")
    fun givenAnErrorOccurredRetrievingConsultations() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatient()

        when (gpSystem) {
            Supplier.EMIS -> {
                mockingClient.forEmis {
                    myRecord.consultationsRequest(patient).respondWithNonDataAccessException()
                }
            }
            Supplier.TPP -> {
                mockingClient.forTpp {
                    myRecord.patientRecordRequest(patient.tppUserSession!!)
                            .respondWithServiceNotAvailableException()
                }

            }
            else -> throw UnsupportedOperationException("Not implemented for this $gpSystem")
        }
    }

    @When("I get the users consultations")
    fun whenIGetTheUsersConsultations() {
        try {
            val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()

            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .myRecord.getMyRecord(patientId)

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

    @Then("^I see Consultations records displayed - Medical Record v1$")
    fun thenISeeConsultationsRecordsDisplayedV1() {
        assertEquals(2, medicalRecordV1Page.consultations.allRecordItems().count())
    }

    @Then("^I see (.*) Consultations records displayed - Medical Record v1$")
    fun thenISeeConsultationsRecordsDisplayedV1(count: Int) {
        assertEquals(count, medicalRecordV1Page.consultations.allRecordItems().count())
    }
}

