package features.myrecord.stepDefinitions

import constants.ErrorResponseCodeTpp
import cucumber.api.java.en.And
import cucumber.api.java.en.But
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.myrecord.factories.MedicationsFactory
import mocking.tpp.models.Error
import net.serenitybdd.core.Serenity
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.assertIsVisible
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers
import worker.models.myrecord.MyRecordResponse

open class MyRecordMedicationsStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Given("^the GP Practice has enabled medications functionality$")
    fun givenTheGPPracticeHasEnabledMedicationsFunctionality() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        MedicationsFactory.getForSupplier(getService).enabledWithRecords(patient)
    }

    @Given("^the GP Practice has enabled medication functionality and the patient has no medications$")
    fun givenTheGPPracticeHasEnabledMedicationsFunctionalityAndPatientHasNoMedications() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        val factory = MedicationsFactory.getForSupplier(getService);
        factory.enabledWithBlankRecord(patient)
        factory.getResult()
    }

    @But("^the GP Practice has disabled medications functionality$")
    fun butTheGPPracticeHasDisabledMedicationsFunctionality() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    myRecord.medicationsRequest(this@MyRecordMedicationsStepDefinitions.patient)
                            .respondWithExceptionWhenNotEnabled()
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

    @Then("^I see current repeat medication information$")
    fun thenISeeCurrentRepeatMedicationInformation() {
        Assert.assertTrue(myRecordInfoPage.repeatMedications.firstElement.element.isVisible)
    }

    @Then("^I see discontinued repeat medication information$")
    fun thenISeeDiscontinuedRepeatMedicationInformation() {
        Assert.assertTrue(myRecordInfoPage.discontinuedRepeatMedications.firstElement.element.isVisible)
    }

    @Then("^I see acute medication information$")
    fun thenISeeAcuteMedicationInformation() {
        myRecordInfoPage.acuteMedications.firstElement.assertIsVisible()
    }
}
