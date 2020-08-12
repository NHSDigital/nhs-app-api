package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.AllergiesFactory
import features.myrecord.factories.MyRecordVisionMocker
import mocking.MockingClient
import mocking.data.myrecord.AllergiesData
import mocking.vision.VisionConstants
import mocking.vision.VisionConstants.allergiesView
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.SerenityHelpers
import worker.models.myrecord.MyRecordResponse

open class MedicalRecordAllergiesStepDefinitionsBackend {

    private val mockingClient = MockingClient.instance

    @Given("^the GP Practice has enabled allergies functionality and the patient has \"(.*)\" allergies$")
    fun givenTheGPPracticeHasEnabledAllergiesFunctionalityAndPatientHasSomeAllergies(count: Int) {
        val gpSystem = SerenityHelpers.getGpSupplier()
        AllergiesFactory.getForSupplier(gpSystem).enabledWithRecords(SerenityHelpers.getPatient(), count)
    }

    @Given("^the GP Practice has enabled allergies functionality and has a drug and non drug allergy " +
            "record for VISION$")
    fun givenTheGPPracticeHasEnabledAllergiesFunctionalityAndThePatientHasADrugAndNonDrugAllergyRecord() {
        MyRecordVisionMocker(mockingClient).generatePatientDataResponse(
                SerenityHelpers.getPatient(),
                allergiesView,
                VisionConstants.htmlResponseFormat)
        { request -> request.respondWithSuccess(AllergiesData.getVisionAllergiesDrugAndNonDrugData()) }
    }


    @Given("^the EMIS GP Practice has two allergies results where the first record has no date$")
    fun givenTheEMISGPPracticeHasTwoAllergiesResultsWhereTheFirstRecordHasNoDate() {
        mockingClient.forEmis.mock {
            myRecord.allergiesRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(AllergiesData.getEmisAllergyRecordsWhereTheFirstRecordHasNoEffectiveDate())
        }
    }

    @Given("the GP Practice has disabled allergies functionality")
    fun givenTheGPPracticeHasDisabledAllergiesFunctionality() {
        AllergiesFactory.getForSupplier(SerenityHelpers.getGpSupplier()).disabled(SerenityHelpers.getPatient())
    }

    @Given("the GP practice returns a bad allergies response")
    fun theGPPracticeReturnsACorruptedResponse(){
        val gpSystem = SerenityHelpers.getGpSupplier()
        AllergiesFactory.getForSupplier(gpSystem).respondWithCorruptedContent(SerenityHelpers.getPatient())
    }

    @Given("^there is an unknown error getting allergies for VISION$")
    fun thereIsAnUnknownErrorGettingAllergiesForVision() {
        MyRecordVisionMocker(mockingClient).generatePatientDataResponse(
                SerenityHelpers.getPatient(),
                allergiesView,
                VisionConstants.htmlResponseFormat )
        { request -> request.respondWithUnknownError() }
    }

    @When("^the flag informing that the patient has access to the allergy data is set to \"(.*)\"$")
    fun andHasAccessToAllergiesDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.allergies.hasAccess)
    }

    @When("^the flag informing that there was an error retrieving the allergy data is set to \"(.*)\"$")
    fun andHasErrorsWhenRetrievingAllergiesDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.allergies.hasErrored)
    }

    @Then("^I receive \"(.*)\" allergies as part of the my record object$")
    fun thenIReceiveAnAllergiesObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.allergies.data.count())
    }
}
