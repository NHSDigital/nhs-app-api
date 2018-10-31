package features.myrecord.stepDefinitions

import constants.ErrorResponseCodeTpp
import cucumber.api.java.en.And
import cucumber.api.java.en.But
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.myrecord.factories.MedicationsFactory
import mocking.tpp.models.Error
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import worker.models.myrecord.MyRecordResponse

open class MyRecordMedicationsStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Then("^I receive the medications object$")
    fun thenIReceiveAMedicationsObject() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertNotNull(result.response.medications.data)
    }

    @Given("^the GP Practice has enabled medications functionality for (.*)$")
    fun givenTheGPPracticeHasEnabledMedicationsFunctionalityfor(getService: String) {
        setPatientToDefaultFor(getService)
        MedicationsFactory.getForSupplier(getService).enabled(this@MyRecordMedicationsStepDefinitions.patient)
    }

    @Given("^the GP Practice has enabled medication functionality and the patient has no medications for (.*)$")
    fun givenTheGPPracticeHasEnabledMedicationsFunctionalityAndPatientHasNoMedicationsFor(getService: String) {
        setPatientToDefaultFor(getService)
        val factory = MedicationsFactory.getForSupplier(getService);
        factory.enabledAndNoMedicationsMock(this@MyRecordMedicationsStepDefinitions.patient)
        factory.getResult()
    }

    @But("^the GP Practice has disabled medications functionality for (.*)$")
    fun butTheGPPracticeHasDisabledMedicationsFunctionalityFor(getService: String) {
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    myRecord.medicationsRequest(this@MyRecordMedicationsStepDefinitions.patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    myRecord.viewPatientOverviewPost(this@MyRecordMedicationsStepDefinitions.patient.tppUserSession!!)
                            .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                                    "Requested record access is disabled by the practice",
                                    "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                }
            }
        }
    }

    @Then("^I receive \"(.*)\" acute medications as part of the my record object$")
    fun thenIReceiveAnAcuteMedicationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.medications.data.acuteMedications.count())
    }

    @Then("^I receive \"(.*)\" current repeat medications as part of the my record object$")
    fun thenIReceiveACurrentRepeatMedicationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.medications.data.currentRepeatMedications.count())
    }

    @Then("^I receive \"(.*)\" discontinued repeat medications as part of the my record object$")
    fun thenIReceiveADiscontinuedRepeatMedicationsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.medications.data.discontinuedRepeatMedications.count())
    }

    @And("^the flag informing that the patient has access to the medications data is set to \"(.*)\"$")
    fun andHasAccessToMedicationsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.medications.hasAccess)
    }

    @And("^the flag informing that there was an error retrieving the medications data is set to \"(.*)\"$")
    fun andHasErrorsWhenRetrievingMedicationsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.medications.hasErrored)
    }
}

